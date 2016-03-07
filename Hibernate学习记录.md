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
还有疑问

- Hibernate 的 cfg.xml 配置文件中有一个 hibernate.use_identifier_rollback 属性, 其默认值为 false, 若把它设为 true, 将改变 delete() 方法的运行行为: delete() 方法会把持久化对象或游离对象的 OID 设置为 null, 使它们变为临时对象.

###evict()###
将session中的一个持久对象移除。