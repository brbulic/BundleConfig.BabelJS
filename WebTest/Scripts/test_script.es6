class Petar {
	constructor(name) {
		this.name = name;
	}

	callMe() {
		return "Hello, " + this.name;
	}
}

var me = new Petar("Bruno");
alert(me.callMe());