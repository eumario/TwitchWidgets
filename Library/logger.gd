class_name Logger extends RefCounted

static var instance : Logger

static var log_level : LogLevel = LogLevel.INFO

enum LogLevel {
	NONE,
	INFO,
	DEBUG,
	WARN,
	ERROR
}

var COLORS : Array[String]= [
	"white",
	"cyan",
	"lightseagreen",
	"goldenrod",
	"red",
]

const LEVEL_NAME : Array[String] = [
	"NONE",
	"INFO",
	"DEBUG",
	"WARN",
	"ERROR",
]

var _log_file : String

func _init(log_file : String) -> void:
	_log_file = log_file
	if FileAccess.file_exists(log_file):
		var modtime := FileAccess.get_modified_time(log_file)
		var base_path := log_file.get_base_dir()
		var file_name := log_file.get_file()
		var dt := Time.get_datetime_dict_from_unix_time(modtime)
		DirAccess.rename_absolute(log_file, base_path.path_join("%02d-%02d-%d_%02d-%02d-%02d_%s" % [
			dt.month,
			dt.day,
			dt.year,
			dt.hour,
			dt.minute,
			dt.second,
			file_name
		]))
	var fh := FileAccess.open(_log_file, FileAccess.WRITE)
	var stamp := Time.get_datetime_dict_from_system()
	fh.store_string("[%02d-%02d-%d - %02d:%02d:%02d] Log File Started\n" % [
		stamp.month,
		stamp.day,
		stamp.year,
		stamp.hour,
		stamp.minute,
		stamp.second,
	])
	fh.flush()
	fh.close()
	instance = self

func log_message(level : LogLevel, message : String) -> void:
	if log_level < level: return
	var stamp := Time.get_datetime_dict_from_system()
	var fh := FileAccess.open(_log_file, FileAccess.READ_WRITE)
	fh.seek_end(0)
	fh.store_string("[%02d-%02d-%d - %02d:%02d:%02d] %s> %s\n" % [
		stamp.month,
		stamp.day,
		stamp.year,
		stamp.hour,
		stamp.minute,
		stamp.second,
		LEVEL_NAME[level],
		message,
	])
	fh.flush()
	fh.close()
	if OS.has_feature("editor"):
		print_rich("[color=green][b][lb]%02d-%02d-%d - %02d:%02d:%02d[rb][/b][/color] [color=%s]%s[/color]: %s" % [
			stamp.month,
			stamp.day,
			stamp.year,
			stamp.hour,
			stamp.minute,
			stamp.second,
			COLORS[level],
			LEVEL_NAME[level],
			message,
		])

static func info(message : String) -> void: instance.log_message(LogLevel.INFO, message)
static func debug(message : String) -> void: instance.log_message(LogLevel.DEBUG, message)
static func warn(message : String) -> void: instance.log_message(LogLevel.WARN, message)
static func error(message : String) -> void: instance.log_message(LogLevel.ERROR, message)
