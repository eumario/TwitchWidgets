@tool
class_name AnimatedTextureRect extends TextureRect

var _currentFrame : int = -1

@export var frames : SpriteFrames :
	get():
		return frames
	set(value):
		frames = value
		if frames == null:
			return
		if is_node_ready():
			var count = frames.get_frame_count(&"default")
			if count > 1:
				play_animation()
			else:
				show_static()

func play_animation() -> void:
	if frames == null:
		_currentFrame = -1
		return
	_currentFrame += 1
	if _currentFrame > frames.get_frame_count(&"default"):
		if not frames.get_animation_loop(&"default"):
			return
		_currentFrame = 0
	var duration = frames.get_frame_duration(&"default", _currentFrame)
	_show_frame(_currentFrame)
	get_tree().create_timer(duration).timeout.connect(play_animation)

func show_static() -> void:
	_show_frame(0)

func _show_frame(frame : int) -> void:
	texture = frames.get_frame_texture(&"default", frame)
