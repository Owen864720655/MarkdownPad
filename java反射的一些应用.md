## 关于反射的理解 ##

java 反射机制可以让我们在编译期(Compile Time)之外的运行期(Runtime)检查类，接口，变量以及方法的信息。反射还可以让我们在运行期实例化对象，调用方法，通过调用 get/set 方法获取变量的值。

要让Java程序能够运行，那么就得让Java类要被Java虚拟机加载。Java类如果不被Java虚拟机加载，是不能正常运行的。现在我们运行的所有的程序都是在编译期的时候就已经知道了你所需要的那个类的已经被加载了。
Java的反射机制是在编译并不确定是哪个类被加载了，而是在程序运行的时候才加载、探知、自审。使用在编译期并不知道的类。这样的特点就是反射。

假如我们有两个程序员，一个程序员在写程序的时候，需要使用第二个程序员所写的类，但第二个程序员并没完成他所写的类。那么第一个程序员的代码能否通过编译呢？这是不能通过编译的。利用Java反射的机制，就可以让第一个程序员在没有得到第二个程序员所写的类的时候，来完成自身代码的编译。
Java的反射机制它知道类的基本结构，这种对Java类结构探知的能力，我们称为Java类的“自审”。大家都用过Jcreator和eclipse。当我们构建出一个对象的时候，去调用该对象的方法和属性的时候。一按点，编译工具就会自动的把该对象能够使用的所有的方法和属性全部都列出来，供用户进行选择。这就是利用了Java反射的原理，是对我们创建对象的探知、自审。


如下列，程序在编译阶段不知道要输出哪个类的信息，在运行期间有用户输入来反射获取
```java
import java.lang.reflect.Constructor;
import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.lang.reflect.Modifier;

import javax.swing.JOptionPane;
/**
 * 本类用于测试反射API，利用用户输入类的全路径， 找到该类所有的成员方法和成员属性
 */
public class ClassTest {
	/**
	 * 构造方法
	 */
	public ClassTest() {
		String classInfo = JOptionPane.showInputDialog(null, "输入类全路径");// 要求用户输入类的全路径
		try {
			Class cla = Class.forName(classInfo);// 根据类的全路径进行类加载，返回该类的Class对象
			Method[] method = cla.getDeclaredMethods();// 利用得到的Class对象的自审，返回方法对象集合
			for (Method me : method) {// 遍历该类方法的集合
				System.out.println(me.toString());// 打印方法信息
			}
			System.out.println("********");
			Field[] field = cla.getDeclaredFields();// 利用得到的Class对象的自审，返回属性对象集合
			for (Field me : field) { // 遍历该类属性的集合
				System.out.println(me.toString());// 打印属性信息
			}
		} catch (ClassNotFoundException e) {
			e.printStackTrace();
		}
	}

	public static void main(String[] args) {
		new ClassTest();
		Class s1 = Customer.class;
		System.out.println(s1.getName());
		System.out.println(s1.getModifiers());
	}
}
```
反射的其他作用参考[http://wiki.jikexueyuan.com/project/java-reflection/](http://wiki.jikexueyuan.com/project/java-reflection/ "java反射机制")
## 利用反射实现动态代理 ##
静态代理与动态代理参考[http://blog.csdn.net/hejingyuan6/article/details/36203505](http://blog.csdn.net/hejingyuan6/article/details/36203505)

利用Java反射机制你可以在运行期动态的创建接口的实现。
你可以通过使用 Proxy.newProxyInstance()方法创建动态代理。 newProxyInstance()方法有三个参数： 1、类加载器（ClassLoader）用来加载动态代理类。 2、一个要实现的接口的数组。 3、一个 InvocationHandler 把所有方法的调用都转到代理上。
```java
InvocationHandler handler = new MyInvocationHandler();
MyInterface proxy = (MyInterface) Proxy.newProxyInstance(
                            MyInterface.class.getClassLoader(),
                            new Class[] { MyInterface.class },
                            handler);
```
### InvocationHandler 接口 ###
在前面提到了当你调用 Proxy.newProxyInstance()方法时，你必须要传入一个 InvocationHandler 接口的实现。所有对动态代理对象的方法调用都会被转向到 InvocationHandler 接口的实现上，下面是 InvocationHandler 接口的定义：
```java
public class MyInvocationHandler implements InvocationHandler{
  public Object invoke(Object proxy, Method method, Object[] args)
  throws Throwable {
    //do something "dynamic"
  }
}
```
传入 invoke()方法中的 proxy 参数是实现要代理接口的动态代理对象。通常你是不需要他的。
invoke()方法中的 Method 对象参数代表了被动态代理的接口中要调用的方法，从这个 method 对象中你可以获取到这个方法名字，方法的参数，参数类型等等信息。关于这部分内容可以查阅之前有关 Method 的文章。

Object 数组参数包含了被动态代理的方法需要的方法参数。注意：原生数据类型（如int，long等等）方法参数传入等价的包装对象（如Integer， Long等等）。
## 实例 ##
定义接口```UserManager```
```java
//接口的方法 增加 删除 查询 修改 操作
public interface UserManager {
	 public void addUser(String userId, String userName);
	 public void delUser(String userId);
	 public String findUser(String userId);
	 public void modifyUser(String userId, String userName);
}

```
定义继承接口的类```UserManagerImpl```
```java
public class UserManagerImpl implements UserManager {

	@Override
	public void addUser(String userId, String userName) {
		System.out.println("UserManagerImpl.addUser");
	}

	@Override
	public void delUser(String userId) {
		System.out.println("UserManagerImpl.delUser");
	}

	@Override
	public String findUser(String userId) {
		System.out.println("UserManagerImpl.findUser");
		return "张三";
	}

	@Override
	public void modifyUser(String userId, String userName) {
		System.out.println("UserManagerImpl.modifyUser");

	}
}
```
通过代理访问```UserManagerImpl```
建立代理类```MyInvocationHandler```实现自```InvocationHandler ```它是把所有的方法调用都转到代理上(即 它里边的invoke里边)
```java
//代理类
public class MyInvocationHandler implements InvocationHandler{
	 private Object target;
//构造函数中传递一个被代理的类的对象,要通过这个对象来调用它的方法
	public MyInvocationHandler(Object target){
		this.target=target;
	};
	   @Override
	    //关联的这个实现类的方法被调用时将被执行  
	    /*InvocationHandler接口的方法，proxy表示代理，method表示原对象被调用的方法，args表示方法的参数*/  
	    public Object invoke(Object proxy, Method method, Object[] args)  
	            throws Throwable {  
	        System.out.println("start-->>");  
	        for(int i=0;i<args.length;i++){  
	            System.out.println(args[i]);  
	        }  
	        Object ret=null;  
	        try{  
	            /*原对象方法调用前处理日志信息*/  
	            System.out.println("satrt-->>");  
	            //调用目标方法  
	            ret=method.invoke(target, args);  
	            /*原对象方法调用后处理日志信息*/  
	            System.out.println("success-->>");  
	        }catch(Exception e){  
	            e.printStackTrace();  
	            System.out.println("error-->>");  
	            throw e;  
	        }  
	        return ret;  
	    }  
}

```
主程序的实现
```java
public class Client2 {

	public static void main(String[] args) {
		//声明代理管理
		InvocationHandler handler = new MyInvocationHandler(new UserManagerImpl());
        //第一个参数指定产生代理对象的类加载器，需要将其指定为和目标对象同一个类加载器  
        //第二个参数要实现和目标对象一样的接口，所以只需要拿到目标对象的实现接口  
        //第三个参数表明这些被拦截的方法在被拦截时需要执行哪个InvocationHandler的invoke方法
		UserManager proxy = (UserManager) Proxy.newProxyInstance(
				UserManagerImpl.class.getClassLoader(),
		                            new Class[] { UserManager.class },
		                            handler);
		proxy.addUser("1111", "张三");
	}

}
```