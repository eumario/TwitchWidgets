class_name TestPlayer extends SQLiteObject

var id : int
var name : String
var exp : float
var level : int

static func setup(dbConn : SQLite) -> void:
	table_name = "players"
	klass = TestPlayer
	db = dbConn
	define_field("id", DataType.INT, Flags.PRIMARY_KEY | Flags.NOT_NULL | Flags.AUTO_INCREMENT)
	define_field("name", DataType.STRING, Flags.NOT_NULL | Flags.DEFAULT, { default = "'Unknown Player'"})
	define_field("exp", DataType.REAL, Flags.NOT_NULL | Flags.DEFAULT, { default = 0.0 })
	define_field("level", DataType.INT, Flags.NOT_NULL | Flags.DEFAULT, { default = 1 })
