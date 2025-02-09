class_name SQLiteObject extends RefCounted

static var table_name : String = ""
static var db : SQLite
static var _table_dictionary : Dictionary = {}
static var klass : GDScript

enum DataType { INT, REAL, STRING, CHAR, BLOB }
const DEFINITION = [
	"int",
	"real",
	"text",
	"char(%d)",
	"blob"
]

enum Flags {
	NONE = 1 << 0,
	NOT_NULL = 1 << 1,
	UNIQUE = 1 << 2,
	DEFAULT = 1 << 3, 
	PRIMARY_KEY = 1 << 4,
	AUTO_INCREMENT = 1 << 5,
	FOREIGN_KEY = 1 << 6,
}

static func define_field(name : String, type : DataType, flags : Flags, extra_params : Dictionary = {}) -> void:
	var definition = {}
	if type == DataType.CHAR and not extra_params.has("size"):
		assert(false, "When defining a CHAR, need to provide a size for the field.")
	if flags & Flags.DEFAULT and not extra_params.has("default"):
		assert(false, "When using flag Default, a default value must be provided.")
	if flags & Flags.FOREIGN_KEY and not extra_params.has("foreign_key"):
		assert(false, "When using Foreign Key, the key name must be provided.")
	
	definition.data_type = DEFINITION[type] if type != DataType.CHAR else DEFINITION[type] % extra_params.size
	if (flags & Flags.NOT_NULL) != 0: definition.not_null = true
	if (flags & Flags.UNIQUE) != 0: definition.unique = true
	if (flags & Flags.DEFAULT) != 0: definition.default = extra_params.default
	if (flags & Flags.AUTO_INCREMENT) != 0: definition.auto_increment = true
	if (flags & Flags.PRIMARY_KEY) != 0: definition.primary_key = true
	if (flags & Flags.FOREIGN_KEY) != 0: definition.foreign_key = extra_params.foreign_key
	_table_dictionary[name] = definition

static func create_table(drop_if_exists : bool = false) -> void:
	assert(db != null, "You need to assign a valid SQLite Database")
	assert(table_name != "", "You need to specify what the table name is supposed to be")
	assert(_table_dictionary.is_empty() != true, "You need to define fields to be used with the database.")
	if table_exists() and drop_if_exists:
		db.drop_table(table_name)
	elif table_exists() and not drop_if_exists:
		assert(false, "Table already exists!")
	
	db.create_table(table_name, _table_dictionary)

static func table_exists() -> bool:
	db.query_with_bindings("SELECT name FROM sqlite_master WHERE type='table' AND name=?;", [table_name])
	return not db.query_result.is_empty()

static func find_one(query : String) -> SQLiteObject:
	var res = db.select_rows(table_name, query, _table_dictionary.keys())
	if res.is_empty():
		return null
	else:
		var row = res[0]
		var obj = klass.new()
		for key in _table_dictionary.keys():
			obj[key] = row[key]
			#obj.set(key, row[key])
		return obj

static func find_many(query : String) -> Array:
	var res = db.select_rows(table_name, query, _table_dictionary.keys())
	if res.is_empty():
		return []
	
	var results : Array[SQLiteObject] = []
	for row in res:
		var obj = klass.new()
		for key in _table_dictionary.keys():
			obj[key] = row[key]
		results.append(obj)
	return results

static func all() -> Array:
	var res = db.select_rows(table_name, "", _table_dictionary.keys())
	if res.is_empty():
		return []
	
	var results : Array[SQLiteObject] = []
	for row in res:
		var obj = klass.new()
		for key in _table_dictionary.keys():
			obj[key] = row[key]
		results.append(obj)
	return results

func row_exists() -> bool:
	assert(_table_dictionary.has("id"), "Unable to check and see if row exists, without an ID key.")
	var res = db.select_rows(table_name, "id = %d" % get("id"), ["id"])
	return not res.is_empty()

func save() -> void:
	if row_exists():
		var data = {}
		for key in _table_dictionary.keys():
			data[key] = get(key)
		db.update_rows(table_name, "id = %d" % get("id"), data)
	else:
		var data = {}
		var primary_autoincrement_key = ""
		for key in _table_dictionary.keys():
			if _table_dictionary[key].has("primary_key"):
				if _table_dictionary[key].has("auto_increment") and self[key] != 0:
					data[key] = self[key]
				if _table_dictionary[key].has("auto_increment"):
					primary_autoincrement_key = key
			else:
				data[key] = self[key]
		db.insert_row(table_name, data)
		if primary_autoincrement_key != null:
			self[primary_autoincrement_key] = db.last_insert_rowid

func delete() -> bool:
	if row_exists():
		return db.delete_rows(table_name, "id = %d" % get("id"))
	return false

func _to_string() -> String:
	var str = "Table: %s (" % table_name
	var vals = []
	for field in _table_dictionary.keys():
		vals.append("%s: %s" % [field, get(field)])
	str += ", ".join(vals) + ")"
	return str
