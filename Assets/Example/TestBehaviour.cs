using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Reflection;
using NLua;

public class TestBehaviour : LuaBaseMonoBehaviour
{
	
	string scriptString = @"
local Lib = {}
function Lib:Start()
	local cube = GameObject.Find('Cube')
	
	local script = cube:GetComponent('LuaBaseMonoBehaviour')
	Debug.Log('script: ' .. tostring(script))

	local r1, r2 = script:test(5, 'abc');
	Debug.Log('result: ' .. tostring(r1) .. ', ' .. tostring(r2))
end
return Lib
";

	LuaTable lib;

	void Awake ()
	{
		InitLuaScriptString (scriptString);

	}

	void Start ()
	{
		Call ("Start");
	}

}
