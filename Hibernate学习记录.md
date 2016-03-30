问题1：刚开始一直不能自动穿件MySql表。是因为hibernate.cfg.xml中的dialect属性设置不对，我自己的引擎是```org.hibernate.dialect.MySQLDialect```不是`org.hibernate.dialect.MySQLInnoDBDialect`2016/3/4 20:51:21 

疑问：hibernate添加事务对于插入，删除的影响，id断层
疑问：<generator class="hilo" />不起作用，和数据库引擎没关系.和自增有没有关系？
疑问：关于delet()方法的删除问题。

Transaction和产生它的session要一起使用，当，session改变之前前一个session的transaction没有提交，再最后提交时就会包错```org.hibernate.ResourceClosedException: org.hibernate.resource.jdbc.internal.LogicalConnectionManagedImpl@4f8969b0 is closed。```
	


##数据的隔离级别
对于同时运行的多个事务, 当这些事务访问数据库中相同的数据时, 如果没有采取必要的隔离机制, 就会导致各种并发问题:  



- 脏读: 对于两个事物 T1, T2, T1 读取了已经被 T2 更新但还没有被提交的字段. 之后, 若 T2 回滚, T1读取的内容就是临时且无效的.  

- 不可重复读: 对于两个事物 T1, T2, T1 读取了一个字段, 然后 T2 更新了该字段. 之后, T1再次读取同一个字段, 值就不同了.  


- 幻读: 对于两个事物 T1, T2, T1 从一个表中读取了一个字段, 然后 T2 在该表中插入了一些新的行. 之后, 如果 T1 再次读取同一个表, 就会多出几行. 

**数据库事务的隔离性**: 数据库系统必须具有隔离并发运行各个事务的能力, 使它们不会相互影响, 避免各种并发问题.   
一个事务与其他事务隔离的程度称为隔离级别. 数据库规定了多种事务隔离级别, 不同隔离级别对应不同的干扰程度, 隔离级别越高, 数据一致性就越好, 但并发性越弱  
数据库提供了4中隔离级别：  
隔离级别    描述  


1. READ UNCOMMITTED(读未提交数据)    允许事务读取未被其他事务提交的变更，脏读、不可重复读和幻读的问题都会出现  


2. READ COMMITED(读已提交数据)   只允许事务读取已经被其他事务提交的变更，可以避免脏读，但不可重复读和幻读问题仍然会出现  


3. REPEATABLE READ(可重复读)   确保事务可以多次从一个字段中读取相同的值，在这个事务持续期间，禁止其他事务对这个字段进行更新，可以避免脏读和不可重复读，但幻读的问题依然存在  
  
4. SERIALIZABLE(串行化)   确保事务可以从一个表中读取相同的行，在这个事务持续期间，禁止其他事务对该表执行插入、更新和删除操作，所有并发问题都可以避免，但性能十分低 。

**Oracl**e 支持的 2 种事务隔离级别：READ COMMITED, SERIALIZABLE. Oracle 默认的事务隔离级别为: READ COMMITED   
**Mysql** 支持 4 中事务隔离级别. Mysql 默认的事务隔离级别为: REPEATABLE READ  


##flush缓存##
**flush** Session 按照缓存中*对象的属性变化*来同步更新数据库
###默认情况下Session刷新缓存

- 显式调用 Session 的 flush() 方法 (可能会发送SQL语句，就是当对象的属性改变时，会调用UPDATE，但不会提交到数据库，也就是数据库中的内容不会改变，因为事务没有提交)

- 当应用程序调用 Transaction 的 commit（）方法的时, 该方法先 flush ，然后在向数据库提交事务

- 其他的情况：执行HQL或QBC查询，会先进行flush()操作，以得到最新的数据

- 如果对象使用 native 生成器生成 OID, 那么当调用 Session 的 save() 方法保存对象时, 会立即执行向数据库插入该实体的 insert 语句.(因为Save方法之后必须保证对象的id是存在的。因为自增，所以必须insert之后才知道id)

##持久化对象的状态##
站在持久化的角度, Hibernate 把对象分为 4 种状态

1. **临时对象（Transient）**: 
	- 在使用代理主键的情况下, OID 通常为 null
	- 不处于 Session 的缓存中
	- 在数据库中没有对应的记录(即new News())
2. **持久化对象(也叫”托管”)（Persist）**：
	- OID 不为 null
	- 位于 Session 缓存中
	- 若在数据库中已经有和其对应的记录, 持久化对象和数据库中的相关记录对应
Session 在 flush 缓存时, 会根据持久化对象的属性变化, 来同步更新数据库
	- 在同一个 Session 实例的缓存中, 数据库表中的每条记录只对应唯一的持久化对象
3. **删除对象(removed)**
	- 在数据库中没有和其 OID 对应的记录
	- 不再处于 Session 缓存中
	- 一般情况下, 应用程序不该再使用被删除的对象
4. **游离对象(也叫”脱管”) （Detached）**
	- OID 不为 null
	- 不再处于 Session 缓存中
	- 一般情况需下, 游离对象是由持久化对象转变过来的, 因此在数据库中可能还存在与它对应的记录

![对象的状态转换图](http://i.imgur.com/xTRBtun.png)

##核心方法##
###Session 的 save() 方法###
Session 的 save() 方法使一个临时对象转变为持久化对象
Session 的 save() 方法完成以下操作:


- 把 News 对象加入到 Session 缓存中, 使它进入持久化状态
选用映射文件指定的标识符生成器, 为持久化对象分配唯一的 OID.
(还有要用该对象时，不会执行select操作)

-  在 使用代理主键的情况下, setId() 方法为 News 对象设置 OID 使无效的.(会报错)


- 计划执行一条 insert 语句：在 flush 缓存的时候


- Hibernate 通过持久化对象的 OID 来维持它和数据库相关记录的对应关系. 当 News 对象处于持久化状态时, 不允许程序随意修改它的 ID

**persist() 和 save() 区别：**
当对一个 OID 不为 Null 的对象执行 save() 方法时, 会把该对象以一个新的 oid 保存到数据库中;  但执行 persist() 方法时会抛出一个异常.即通过setId方法设定了一个OID再次插入时persist()会报错，而save()方法会以一个新的OID保存到数据库中.
###Session 的 get() 和 load() 方法###
都可以根据跟定的 OID 从数据库中加载一个持久化对象
区别:


- 当数据库中不存在与 OID 对应的记录时,且Session没有被关闭 load() 方法抛出 ObjectNotFoundException 异常, 而 get() 方法返回 null(因为load返回的是一个代理对象，相当于已经答应别人的事做不了了，抛异常)


- 两者采用不同的延迟检索策略：load 方法支持延迟加载策略。而 get 不支持。
执行get方法会立即加载对象，而load方法，若不适用该对象，则不会立即检索，而是返回一个代理对象


- Load方法可能会 抛出LazyInitializationException异常（在需要初始化代理对象之前已经关闭了Session,即要真正使用查询结果时，连接已经关掉了，因为load延迟加载)

###update()方法###
1. 若更新一个持久化对象，则不需要显示的调用update()方法，因为在commit()中会执行flush()操作.
2. 更新一个游离对象，需要显示的调用update()方法，并把该对象转变为持久化对象.(游离对象，即session被重新赋值了，不是以前的session了这个时候该对象就变为一个游离对象)

注意：

1. 无论要更新的游离对象和数据表中的数据是否一样，都会发送update语句
若希望 Session 仅当修改了 News 对象的属性时, 才执行 update() 语句, 可以把映射文件中 <class> 元素的 select-before-update 设为 true. 该属性的默认值为 false(update绑定触发器的时候会用)

2. 当 update() 方法关联一个游离对象时, 如果在数据库中不存在相应的记录, 也会抛出异常. 

3. 当 update() 方法关联一个游离对象时, 如果在 Session 的缓存中已经存在相同 OID 的持久化对象, 会抛出异常(也就是在执行update方法时，要把游离对象编程持久对象，但这个时候session中已经存在了一个同样id的对象，就会报错)

###saveOrUpdate()方法###
![](http://i.imgur.com/yUhybo6.png)

```java

		//临时对象
		News news = new News("哈气", "感冒了，困死了", new Date(new java.util.Date().getTime()));
		//有id但是不在Session中，是一个游离对象
		news.setId(1);
		session.saveOrUpdate(news);//执行update方法如果，数据表中存在id为1的数据，不存在的话会报错
		news.setId(2);//会报错，因为update之后对象已经编程持久对象，持久对象不允许改id
```
###merge() 方法###
遗留问题

###delete()方法###
- Session 的 delete() 方法既可以删除一个游离对象, 也可以删除一个持久化对象
**Session 的 delete() 方法处理过程**
	- 计划执行一条 delete 语句
	- 把对象从 Session 缓存中删除, 该对象进入删除状态.(在flush缓存时发送delete语句中间这段时间再想对该对象进行 save或者update时就会报错，解决办法：下面)
**还有疑问**

- Hibernate 的 cfg.xml 配置文件中有一个 hibernate.use_identifier_rollback 属性, 其默认值为 false, 若把它设为 true, 将改变 delete() 方法的运行行为: delete() 方法会把持久化对象或游离对象的 OID 设置为 null, 使它们变为临时对象.

###evict()###
将session中的一个持久对象移除。 
### clear()###
把缓冲区内的全部对象清除，但不包括操作中的对象。(疑问，操作中的状态指什么是不是如下)
```java
		News news = session.load(News.class, 2);
		System.out.println(news);
		session.clear();
		news = session.load(News.class, 2);
		System.out.println(news);
``` 
只发送了一次查询操作，evict也是同样道理？

##Java 时间和日期类型的 Hibernate 映射##
在 Java 中, 代表时间和日期的类型包括: java.util.Date 和 java.util.Calendar. 此外, 在 JDBC API 中还提供了 3 个扩展了 java.util.Date 类的子类: java.sql.Date, java.sql.Time 和 java.sql.Timestamp, 这三个类分别和标准 SQL 类型中的 DATE, TIME 和 TIMESTAMP 类型对应

![Java 时间和日期类型的 Hibernate 映射](http://i.imgur.com/x50G7QZ.png)
持久化类中设置为java.util.Date，然后数据库中具体需要那种类型的根据映射类型在hbm.xml中设置
插入的时候```new Date(new java.util.Date().getTime())``` 
###存储图片
图片存储为2进制格式。Blob
在持久化类中声明private Blob image;
InputStream stream = new FileInputStream("tp.png");
```java
		Blob image = Hibernate.getLobCreator(session).createBlob(stream, stream.available());
		news.setImage(image);
```
##映射组成关系##
**Hibernate 把持久化类的属性分为两种: **
- 值(value)类型: 没有 OID, 不能被单独持久化, 生命周期依赖于所属的持久化类的对象的生命周期

- 实体(entity)类型: 有 OID, 可以被单独持久化, 有独立的生命周期
即把一个表中一部分分离出来，提高可重用行。

```xml
<component name="pay" class="Pay">
			<parent name="worker" />
			<property name="mouthPay" column="MOUTH_PAY" type="integer"></property>
			<property name="yearPay" column="YEAR_PAY" type="integer"></property>
		</component>
```
##映射单向多对一关联关系##

只需从 n 的一端可以访问 1 的一端

在多的一边.hbm.xml中配置
```xml
<many-to-one name="customer" class="com.yth.hibernate.map.Customer" >
            <column name="CUSTOMER_ID" />
        </many-to-one>
```
column: 设定和持久化类的属性对应的表的外键

**注意事项**
1. 若查询多的一端的对象，默认情况下是不会查询所关联的1的那一端的对象，只有在用到关联对象时才发送相应的SQL语句.延迟加载

2. 若在多的一端来查询1的一端时，如果此时session已经关闭，则这个时候会发生LazyInitializationException异常。
 
3. 获取多的一端对象时，默认情况下，1的一端是一个代理对象.

4. 如果1端关联的对象还都在，则不能删除1端。

##双向1-n##
域模型:从 Order 到 Customer 的多对一双向关联需要在Order 类中定义一个 Customer 属性, 而在 Customer 类中需定义存放 Order 对象的集合属性
在多的一头配置和单向多对一一样。
在一的一端配置
```xml
<set name="orders" table="ORDERTEST" inverse="true">
			<key column="CUSTOMER"></key>
			<one-to-many class="OrderTest"/>
		</set>
```
name：设置持久化类中Set对象名
table:n端的表名
inverse：值为true为被动方，说明负责关联表的关系的是n端。
key: 设置与所关联的持久化类对应的表的外键，column指定关联表的外键名称。
one-to-many:1对多，设定集合属性中所关联的持久化类 class为N端全类名
注意：
1. 声明集合类型时，需要使用接口类型（Set）因为hibernate在获取集合类型时是hibernate内置的集合类型，而不是java标准的类型。该类型具有**延迟加载和代理对象**的作用。

2. 需要在声明时对集合进行初始化，方式包空指针异常。

3. 两边都是延迟加载，都是在用到对方时才发送SELECT语句

###Set的其他属性###
order-by:""对查到的集合属性进行排序。可以加入sql函数
##一对一##
配置many-to-one的一端，都有一个外键。插入的时候先插入另一端再插入many端，这样不会发送update语句。查询的时候，只查询本对象，所关联的对象只有当用的时候才会发送select语句
<many-to-one属性的类
###基于外键的###

一端配置
```xml
<many-to-one name="manager" class="Manager">
			<column name="MANAGE_ID" unique="true"></column>
		</many-to-one>
```
增加unique=“true” 属性来表示为1-1关联
另一端
```xml
<one-to-one name="department" class="Department" property-ref="mag"></one-to-one>
```
用 property-ref 属性指定使用被关联实体主键以外的字段作为关联字段(就是指定关联字段 ,所关联表的和他同类的对象名称)
就是上面，Department中Manager对象名称。

注意：
查询操作时，配置many-to-one的一端，在用到所关联的对象时才会发送SELECT语句，但是配置了one-to-one一端的只要查询了就会把他所关联的对象全部查询出来。
###基于主键的###
基于主键的映射策略:指一端的主键生成器使用 foreign 策略,表明根据”对方”的主键来生成自己的主键，自己并不能独立生成主键. <param> 子元素指定使用当前持久化类的哪个属性作为 “对方”
```xml
<id name="id" type="java.lang.Integer">
			<column name="ID" />
			<!-- foreign根据对方的主键来生成自己的主键 param标示使用当前持久化类的哪个属性作为对方 即根据哪个来生成主键 -->
			<generator class="foreign">
			<param name="property">manager</param>
			</generator>
		</id>
```
并在当前的.hbm.xml中配置one-one
```xml
<one-to-one name="manager" class="Manager" constrained="true"></one-to-one>
```
one-to-one属性还应增加 constrained=“true” 
constrained(约束):指定为当前持久化类对应的数据库表的主键添加一个外键约束，引用被关联的对象(“对方”)所对应的数据库表主键

另一端就是配置
```xml
<one-to-one name="department" class="Department"></one-to-one>
```
注意：
1. 在插入时都是先插入标准配置的，即主动生成主键的一方。因为另一方没有主键
2. 在查询时，主动的一方会有一个左外连接把所关联的对象也都查出来，因为他里面没有一个外键指向所关联的对象，不知道是哪一个，所有都查出来了。
3. 被动的一方默认只查询他所在的表，只有用到所关联的表时才会全部查询.

##多对多##
例子：现实生活中的类别和商品

n-n双向配置。
代码
```xml
 <set name="category" table="CATEGORY_ITEM" inverse="true">
            <key>
            <!-- 这个 配置的是category端在中间表中的列名-->
                <column name="I_ID" />
            </key>
            <!-- 配置的是Item在中间表中的列的名称 -->
            <many-to-many class="Category" column="C_ID"></many-to-many>
        </set>
```
另一端
```xml
 <set name="items" table="CATEGORY_ITEM">
            <key>
            <!-- 这个 配置的是category端在中间表中的列名-->
                <column name="C_ID" />
            </key>
            <!-- 配置的是Item在中间表中的列的名称 -->
            <many-to-many class="Item" column="I_ID"></many-to-many>
        </set>
```
**set中table**属性是连接表的表名，必须一样
**key:colum**属性指该类对应的表在 连接表中的列属性名.
**many-to-many**配置，Set中类的对象.column属性用于指定，他在连接表中列属性名
**inverse="true"**在一端中设置他来维护关联关系.
两个表的配置，交叉相同

如果要配置单向的n-n，只用在一个.hbm.xml中配置set即可

##映射的继承关系##

##HQL查询##
HQL 检索方式包括以下步骤:
1. 通过 Session 的 createQuery() 方法创建一个 Query 对象, 它包括一个 HQL 查询语句. HQL 查询语句中可以包含命名参数
2. 动态绑定参数
3. 调用 Query 相关方法执行查询语句. 

```java
		 //建立hal语句
		 String hql = "FROM Class c WHERE c.id=?";
		// 创建query
		 Query query = session.createQuery(hql);
		// 设置query参数
		 query.setInteger(0, 2);
		// 查询
		 java.util.List<Class> ces = query.list();
```


###HQL vs SQL:###
HQL 查询语句是面向对象的, Hibernate 负责解析 HQL 查询语句, 然后根据对象-关系映射文件中的映射信息, 把 HQL 查询语句翻译成相应的 SQL 语句. **HQL 查询语句中的主体是域模型中的类及类的属性
SQL 查询语句是与关系数据库绑定在一起的. SQL 查询语句中的主体是数据库表及表的字段.** 
##在映射文件中定义命名查询语句##
Hibernate 允许在映射文件中定义字符串形式的查询语句. 也就是把HQL语句放到映射文件中。

<query> 元素用于定义一个 HQL 查询语句, 它和 <class> 元素并列. 
```xml
<query name="studentSearch"><![CDATA[FROM Student st WHERE st.id>=:studentId ]]></query>
```
在程序中通过 Session 的``` getNamedQuery() ```方法获取查询语句对应的 Query 对象. 
```java
Query query = session.getNamedQuery("studentSearch");

		java.util.List<Student> stes = query.setInteger("studentId", 5).list();

		System.out.println(stes.get(0).getName());
```
##投影查询##
**投影查询**: 查询结果仅包含实体的部分属性. 通过 SELECT 关键字实现

Query 的 list() 方法返回的集合中包含的是数组类型的元素, 每个对象数组代表查询结果的一条记录
查询多个值：
```java
		String hql = "SELECT st.name,st.age FROM Student st WHERE st.id>:studentId ";

		Query query = session.createQuery(hql);

		java.util.List<Object[]> result = query.setInteger("studentId", 20).list();

		System.out.println(result.size());

		for( Object[] obj: result){
			System.out.println(Arrays.asList(obj));
		}
```

查询单个值
```java
		String hql = "SELECT st.name FROM Student st WHERE st.id>:studentId ";
		Query query = session.createQuery(hql);

		java.util.List<Object[]> result = query.setInteger("studentId", 20).list();

		System.out.println(result.size());

		for( Object obj: result){
			System.out.println(Arrays.asList(obj));
		}
```
##疑问
查询单个值时List里边设为Object[]或者Object都可以，但是for循环里边必须是Obj
而查询多个值时List和for循环里边都必须是Object[];

可以在持久化类中定义一个**对象的构造器**来包装投影查询返回的记录, 使程序代码能完全运用面向对象的语义来访问查询结果集. 
```java
		String hql = "SELECT new Student(st.name,st.age) FROM Student st WHERE st.id>:studentId ";
		Query query = session.createQuery(hql);
		java.util.List<Student> result = query.setInteger("studentId", 20).list();
		System.out.println(result.size());
		for (Student obj : result) {
			System.out.println(obj.getName()+" 年龄"+obj.getAge());
		}
```
如果要获取没有查询的属性就会返回null比如上面的```obj.getId()```

##报表查询##
报表查询用于对数据分组和统计, 与 SQL 一样, HQL 利用 **GROUP BY** 关键字对数据分组, 用 **HAVING** 关键字对分组数据设定约束条件.
在 HQL 查询语句中可以调用以下聚集函数
- count()
- min()
- max()
- sum()
- avg()
 
```java
String hql = "SELECT min(st.age),max(st.age)FROM Student st GROUP BY st.className " + "HAVING min(st.age)>20";
		Query query = session.createQuery(hql);
		java.util.List<Object[]> result = query.list();
		for (Object[] obj : result) {
			System.out.println(Arrays.asList(obj));
		}
```
##左外连接/迫切左外连接##
###迫切左外连接:
**LEFT JOIN FETCH **关键字表示迫切左外连接检索策略.
list() 方法返回的集合中存放**实体对象的引用**, 每个 Class 对象关联的 Employee  集合都被初始化, 存放所有关联的 Student 的实体对象. 
```java
		String hql = "SELECT DISTINCT c FROM Class c LEFT JOIN FETCH c.students";
		Query query = session.createQuery(hql);
		java.util.List<Class> c = query.list();
		for (Class s : c) {
			for (Student st : s.getStudents()) {
				System.out.println(st.getName());
			}
		}
```
查询结果中可能会包含重复元素
###左外连接
- LEFT JOIN 关键字表示左外连接查询.  
- list() 方法返回的集合中存放的是对象数组类型
- 根据配置文件来决定 Employee 集合的检索策略. fetch/lazy..
- 如果希望 list() 方法返回的集合中仅包含 Class 对象, 可以在HQL 查询语句中使用 SELECT 关键字
//只查询Class对象
```java
String hql = "SELECT DISTINCT c FROM Class c LEFT JOIN c.students";
		Query query = session.createQuery(hql);
		java.util.List<Class> result = query.list();
		for (Class obj : result) {
			System.out.println(obj.getName()+" 学生数"+obj.getStudents().size());
		}

```
//默认情况下返回的是一个对象数组
```java
String hql = "FROM Class c LEFT JOIN c.students";
		Query query = session.createQuery(hql);
		java.util.List<Object[]> result = query.list();
		for (Object[] obj : result) {
			System.out.println(Arrays.asList(obj));
		}
```
##迫切做内连接
###迫切内连接 ###
(不包括左表不符合条件的记录):
INNER JOIN FETCH 关键字表示迫切内连接, 也可以省略 INNER 关键字
list() 方法返回的集合中存放** Department 对象**的引用, 每个 Department 对象的 Employee 集合都被初始化, 存放所有关联的 Employee 对象

###内连接:

INNER JOIN 关键字表示内连接, 也可以省略 INNER 关键字
list() 方法的集合中存放的每个元素对应查询结果的一条记录, 每个元素都**是对象数组类型**
如果希望 list() 方法的返回的集合仅包含 Department  对象, 可以在 HQL 查询语句中使用 SELECT 关键字

##QBC 检索


##疑问##
取名Order的表映射不上