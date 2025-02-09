@tool
extends EditorScript

func _run() -> void:
	var db = SQLite.new()
	db.path = "user://test_db"
	#db.verbosity_level = SQLite.VERBOSE
	db.open_db()
	TestPlayer.setup(db)
	TestPlayer.create_table(true)
	var player = TestPlayer.new()
	player.name = "CasperDragonWolf"
	player.level = 10
	player.exp = 5000
	player.save()
	player = TestPlayer.new()
	player.name = "casper031584"
	player.level = 5
	player.exp = 2500
	player.save()
	
	var newPlayer : TestPlayer = TestPlayer.find_one("name='CasperDragonWolf'")
	if newPlayer == null:
		print("Failed to get player")
	else:
		print(player)
		print(newPlayer)
	
	var players : Array[TestPlayer] = []
	players.assign(TestPlayer.all())
	print(players)
	
	print("Delete %s: " % players[1].name, players[1].delete())
	players.assign(Player.all())
	print(players)
	
	db.close_db()
