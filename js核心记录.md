# JavaScript核心 #

----------
## 一JS自定义对象及属性 ##
 1. 定义对象属性

 JS中可以为对象定义三种类型的属性(这个和java类似)

- 私有属性(相当于类中的private) 
私有属性只能在构造函数内部定义与使用
```javascript
function  User(age) {
    this.age=age;
    var isLarger=age>12;
    this.Up=isLarger;
}
```

- 实例属性 (相当于类中的publice)

```javascript
//语法结构 
//1.functionName.prototype.propertyName=value
//2.this.propertyName=value
```
- 类属性(相当于类中的 public static)
```javascript
//语法结构
functionName.propertyName=value
//例如
User.AGE=23;
```

2.定义对象方法
同对象属性 js中可以为对象定义三种方法
- 私有方法
```js
  function User(name){
    this.name=name;
    function getNameLength(nameStr){
      return nameStr.length;
    }
    this.nameLength=getNameLength(this.name); 
   }
```
私有方法必须在构造函数体内定义，而且只能在构造函数体内使用。

- 实例方法 
同样式两种方法
```js
 functionName.prototype.methodName=method;
 functionName.prototype.methodName=function(arg1,…,argN){};

 this.methodName=method;
 this.methodName=function(arg1,…,argN){};

//例如
    function User(name){
        this.name=name;
        this.getName=getUserName;
        this.setName=setUserName;
    }
    
    function getUserName(){
        return this.name;
    }
    
    function setUserName(name){
        this.name=name;
    }
    var user=new User('yth');
    user.setName('ly');
    console.log(user.getName());
```
- 类方法  类方法需要在构造函数外面定义(**可以在任何地方使用，但不能通过对象的实例来引用**)
```js
//例子：
   function User(name){
      this.name=name;
   }

  User.getMaxAge=getUserMaxAge;

  function getUserMaxAge(){
     return 200;
   }

//或者
  User.getMaxAge=function(){return 200;};
  alert(User.getMaxAge());
```

3.属性与方法的动态增加和删除
```js
动态增加对象属性
  obj.newPropertyName=value;
动态增加对象方法
  obj.newMethodName=method或者=function(arg1,…,argN){}
动态删除对象属性
  delete obj.propertyName
动态删除对象方法
  delete obj.methodName
```

4. 对象的创建
	- 对象初始化器方式
	```js 
	var user={name:'yth',age:23,sex:'男'}
	```
	- 构造函数方式
	```js
	function User(name, age, sex) {
	    this.name = name;
	    this.age = age;
	    this.sex = sex;
	}
	var user2=new User('ly',20,'女');
```

> 注：函数只要是要调用它进行执行的，都必须加括号。此时，函数()实际上等于函数的返回值。当然，有些没有返回值，但已经执行了函数体内的行为，这个是根本，就是说，只要加括号的，就代表将会执行函数体代码。
> 不加括号的，都是把函数名称作为函数的指针，用于传参，此时不是得到函数的结果，因为不会运行函数体代码。



----------

##  二 js中函数的定义与函数作用域 ##

1. 函数声明
函数就是一段可以反复调用的代码块。函数声明由三部分组成：函数名，函数参数，函数体。整体的构造是function命令后面是函数名，函数名后面是一对圆括号，里面是传入函数的参数。函数体放在大括号里面。当函数体没有使用return关键字时，函数调用时返回默认的undefined；如果有使用return语句，则返回指定内容。函数最后不用加上冒号。

> 函数声明是在预执行期执行的，也就是说函数声明是在浏览器准备解析并执行脚本代码的时候执行的。所以，当去调用一个函数声明时，可以在其前面调用并且不会报错。

```js
 console.log(rascal()) // 'rascal'
 function rascal(){
   return 'rascal';
 }
```

2. 函数表达式
函数表达式是把一个匿名函数赋给一个全局变量。这个匿名函数又称为函数表达式
```js
 var keith = function() {
   //函数体
 };
```
**注意** :函数表达式与函数声明不同的是，函数表达式是浏览器解析并执行到那一行才会有定义。也就是说，不能在函数定义之前调用函数。函数表达式并不像函数声明一样有函数名的提升。如果采用赋值语句定义函数并且在声明函数前调用函数，JavaScript就会报错。
```js
 keith();
 var keith = function() {};
//报错 TypeError: keith is not a function

//上面的代码相当于   keith只被声明了，还没有被赋值
 var keith;
 console.log(keith()); // TypeError: keith is not a function
 keith = function() {};
```

3. 命名函数的函数表达式  (该函数名只在函数体内部有效，在函数体外部无效。)
```js
 var keith = function boy(){
  console.log(typeof boy);
 };
```

----------

## js中函数参数与闭包 ##

### 参数传递方式 ###

当函数参数是原始数据类型时（**字符串**，**数值**，**布尔值**），参数的传递方式为传值传递。也就是说，在函数体内修改参数值，不会影响到函数外部。

但是，如果函数参数是复合类型的值（数组、对象、其他函数），传递方式是传址传递（pass by reference）。也就是说，传入函数的是原始值的地址，因此在函数内部修改参数，将会影响到原始值。  (注意，如果函数内部修改的，不是参数对象的某个属性，而是替换掉整个参数，这时不会影响到原始值。) 这是因为，形式参数（Arr）与实际参数arr存在一个赋值关系。
```js
	var array=[1,2,3];
	function test(arr) {
	    arr[0]=0;
	}
	test(array);
	console.log(array[0]);//此处输出0


	var array=[1,2,3];
	function test(arr) {
	    arr=[0,4,5];//即便是arr=[0,2,3]也不会改
	}
	console.log(array[0]);
	test(array);
	console.log(array[0]);
```
### js中的闭包 ###
闭包就是函数rascal，即能够读取其他函数内部变量的函数。由于在JavaScript语言中，只有函数内部的子函数才能读取内部变量，因此可以把闭包简单理解成“定义在一个函数内部的函数”。闭包最大的特点，就是它可以“记住”诞生的环境，比如rascal记住了它诞生的环境keith，所以从rascal可以得到keith的内部变量。
```js
	 function keith(){
	   var a=1;
	   function rascal(){
	     return a;
	   }
	   return rascal;
	 }
	 var result=keith();
	 console.log(result()); //1
	//---------------------------------------------
	 function keith(){
	   var a=1;
	   return function(){
	     return a;
	    };
	 }
	 var result=keith();
	 console.log(result()) //1
```
> 通过以上的例子，总结一下闭包的特点：
> 
> 　　1：在一个函数内部定义另外一个函数，并且返回这个内部函数。
> 
> 　　2：内部函数可以读取外部函数定义的局部变量
> 
> 　　3：让局部变量始终保存在内存中。也就是说，闭包可以使得它诞生环境一直存在。





无论函数是在哪里调用，也无论函数是如何调用的，其确定的词法作用域永远都是在函数被声明的时候确定下来的。理解这一点非常重要。

变量名提升:
```js
 var a = 2;
以上代码其实会分为两个过程，一个是 var a; 一个是 a = 2;  其中var a;是在编译过程中执行的，a =2是在执行过程中执行的。理解了这个，那么你就应该知道下面为何是这样的结果了:
```

> 所以在编译阶段，编译器会将函数里所有的声明都提前到函数体内的上部，而真正赋值的操作留在原来的位置上，这也就是上面的代码打出undefined的原因。需要注意的是，变量名提升是以函数为界的，嵌套函数内声明的变量不会提升到外部函数体的上部。









//问题:













## 三 js中的this指向

![JavaScript this 指向图](http://i.imgur.com/iLSDJXS.png);

。一般在编译期绑定。而 JavaScript 中this 在运行期进行绑定的，这是JavaScript 中this 关键字具备多重含义的本质原因。

　　
> “JavaScript 中的函数既可以被当作普通函数执行，也可以作为对象的方法执行，这是导致this含义如此丰富的主要原因。一个函数被执行时，会创建一个执行环境（ExecutionContext），函数的所有的行为均发生在此执行环境中，构建该执行环境时，JavaScript首先会创建arguments变量，其中包含调用函数时传入的参数。接下来创建作用域链。然后初始化变量，首先初始化函数的形参表，值为arguments变量中对应的值，如果arguments变量中没有对应值，则该形参初始化为undefined。如果该函数中含有内部函数，则初始化这些内部函数。如果没有，继续初始化该函数内定义的局部变量，需要注意的是此时这些变量初始化为undefined，其赋值操作在执行环境（ExecutionContext）创建成功后，函数执行时才会执行，这点对于我们理解JavaScript中的变量作用域非常重要，鉴于篇幅，我们先不在这里讨论这个话题。最后为this变量赋值，如前所述，会根据函数调用方式的不同，赋给this全局对象，当前对象等。至此函数的执行环境（ExecutionContext）创建成功，函数开始逐行执行，所需变量均从之前构建好的执行环境（ExecutionContext）中读取。”

### JavaScript中bind、call、apply函数用法详解 ###
js里函数调用有4种模式：方法调用、正常函数调用、构造器函数调用、apply/call 调用。
-  方法调用
```js
  var a = { 
   v : 0, 
   f : function(xx) { 
        this.v = xx; 
       } 
  } 
  a.f(5);// 对象的创建，对象初始化器方法（还有一种是构造函数方式）
```
- 正常函数调用
```js
  function f(xx) { 
    this.x = xx; 
  } 
  f(5);//这里的this指的是全局变量window.x(根据上面的this链来判断)
```
- 构造器函数调用
```js
  function a(xx)
  { 
    this.m = xx; 
   } 
  var b = new a(5);
  //this指b
```
- 四、apply/call 调用：
  上面的3种函数调用方式，你可以看到，this都是自动绑定的，没办法由你来设，当你想设的时候，就可以用apply()了。apply函数接收2个参数，第一个是传递给这个函数用来绑定this的值，第二个是一个参数数组。

> 如果你apply的第一个参数传递null，那么在函数a里面this指针依然会绑定全局对象

```js
  function a(xx) { 
   this.b = xx; 
  } 
  var o = {}; 
  a.apply(o, [5]); 
  alert(a.b); // undefined 
  alert(o.b); // 5
```
call()呢，它的第一个参数也是绑定给this的值，但是后面接受的是不定参数，而不再是一个数组，也就是说你可以像平时给函数传参那样把这些参数一个一个传递。
```js
  function a(xx, yy) { 
    alert(xx, yy); 
    alert(this); 
    alert(arguments); 
  } 
  a.apply(null, [5, 55]); 
  a.call(null, 5, 55);
```
bind()函数，上面讲的无论是call()也好， apply()也好，都是立马就调用了对应的函数，而bind()不会， bind()会生成一个新的函数，bind()函数的参数跟call()一致，第一个参数也是绑定this的值，后面接受传递给函数的不定参数
```js
   var m = { 
    "x" : 1 
   }; 
   function foo(y) { 
     alert(this.x + y); 
   } 

  foo.apply(m, [5]); 
  foo.call(m, 5); 
  var foo1 = foo.bind(m, 5); 
  foo1();
```