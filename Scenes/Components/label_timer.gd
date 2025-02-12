@tool
class_name LabelTimer extends Label

var _timer : Timer

@export var countdown : bool = false
@export var start_count : int = 600
@export var countdown_complete_text : String = "Boom!"
@export var autostart : bool = false
@export var fuzzy_time : bool = false

var _cur_time : float = 0.0

func _init() -> void:
	_timer = Timer.new()
	_timer.autostart = false
	_timer.one_shot = false
	add_child(_timer)

func _ready() -> void:
	if countdown:
		_cur_time = start_count
	else:
		_cur_time = 0.0
	
	if autostart: start()
	_timer.timeout.connect(func():
		_cur_time += -1 if countdown else 1
		if _cur_time < 0:
			_timer.stop()
	)

func _process(delta: float) -> void:
	if _timer.is_stopped():
		if countdown and text != countdown_complete_text:
			text = countdown_complete_text
		return
	
	if fuzzy_time:
		text = Utils.lazy_time(_cur_time)
	else:
		text = Utils.format_time(_cur_time)

func start() -> void:
	_timer.start(1)
