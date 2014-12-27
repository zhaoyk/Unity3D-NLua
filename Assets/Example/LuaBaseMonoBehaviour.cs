using UnityEngine;
using System.Collections;
using NLua;

public class LuaBaseMonoBehaviour : MonoBehaviour
{
	private static Lua lua;

	public static Lua Lua ()
	{
		return lua;
	}

	private static string initString = @"
import 'System'
import 'UnityEngine'
import 'Assembly-CSharp'
";

	static LuaBaseMonoBehaviour ()
	{
		lua = new Lua ();
		lua.LoadCLRPackage ();
		lua.DoString (initString);
	}

	private LuaTable luaObject;

	public void InitLuaScriptFile (string file)
	{
		object[] obj = DoFile (file);
		InitResult (obj);
	}

	public void InitLuaScriptString (string str)
	{
		object[] obj = DoString (str);
		InitResult (obj);
	}

	private void InitResult (object[] obj)
	{
		if (obj != null && obj.Length > 0) {
			luaObject = obj [0] as LuaTable;
		}
	}

	public object[] DoString (string str)
	{
		return lua.DoString (str);
	}

	public object[] DoFile (string filename)
	{
		TextAsset txt = Resources.Load<TextAsset> (filename);
		return lua.DoString (txt.text);
	}

	public System.Object[] Call (string function, params System.Object[] args)
	{
		System.Object[] result = new System.Object[0];
		if (luaObject == null)
			return result;
		LuaFunction lf = luaObject [function] as LuaFunction;
		if (lf == null)
			return result;
		try {
			// Note: calling a function that does not 
			// exist does not throw an exception.
			if (args != null) {
				object[] _args = new object[args.Length + 1];
				_args [0] = this;
				for (int i = 0; i < args.Length; ++i) {
					_args [i + 1] = args [i];
				}
				result = lf.Call (_args);
			} else {
				result = lf.Call (this);
			}
		} catch (NLua.Exceptions.LuaException e) {
			Debug.LogError (FormatException (e), gameObject);
		}
		return result;
	}

	public System.Object[] Call (string function)
	{
		return Call (function, this);
	}
	
	public static string FormatException (NLua.Exceptions.LuaException e)
	{
		string source = (string.IsNullOrEmpty (e.Source)) ? "<no source>" : e.Source.Substring (0, e.Source.Length - 2);
		return string.Format ("{0}\nLua (at {2})", e.Message, string.Empty, source);
	}

	virtual public object[] MethodMissing (string methodName, params object[] args)
	{
		return Call (methodName, args);
	}

}
