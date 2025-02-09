extends MarginContainer

func _ready() -> void:
	%ClientId.text = Managers.settings.data.client_id
	%ClientSecret.text = Managers.settings.data.client_secret
	
	%Authorize.pressed.connect(func():
		var id = %ClientId.text
		var secret = %ClientSecret.text
		
		if id == "" and secret == "":
			print("Need to provide a ID and Secret")
			return
		
		Managers.settings.data.client_id = id
		Managers.settings.data.client_secret = secret
		
		Managers.twitch.setup_auth_info(id, secret)
	)
