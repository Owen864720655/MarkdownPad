```java
package com.yth;
import java.io.FileNotFoundException;
import java.io.FileReader;
import javax.script.Invocable;
import javax.script.ScriptEngine;
import javax.script.ScriptEngineManager;
import javax.script.ScriptException;

public class JsCall {
	public int sum(int a, int b)  {
		ScriptEngineManager scriptEngineManager = new ScriptEngineManager(); 
	      ScriptEngine nashorn = scriptEngineManager.getEngineByName("nashorn"); 
	      try {
			nashorn.eval(new FileReader("l.js"));
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		} catch (ScriptException e) {
			e.printStackTrace();
		}
	      Invocable invocable = (Invocable) nashorn;
		//接口的引用 接口里边的方法具体实现由js里边的同名方法来实现
	      Adder adderaa = invocable.getInterface(Adder.class);
		return adderaa.sum(a,b);
	}
}

```
```java
package com.yth;

public interface Adder {
	int sum(int a, int b);
}

```
调用接口来实现js里边的方法