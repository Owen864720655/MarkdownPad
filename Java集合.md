# java集合 #
------------
java数组的长度是固定的，且在同一个数组中只能存放相同类型的数据，数组可以存放基本类型，也能存放引用类型的数据。
为了方便的存储和操作数目不固定的一组数据，JDK提供了Java集合。

----------

![](http://i.imgur.com/lAlXHbY.png)

----------


![](http://i.imgur.com/BleHBQZ.png)

----------

Collection接口声明了java集合（set和list）的通用方法
![](http://i.imgur.com/wusR1dG.png)

----------
## Iterator接口 ##

Iterator接口向客户端提供了边里各种类型的集合的统一接口。
- hasNext():判断集合中的元素是否遍历完毕，如果没有就返回true
- next():返回下一个元素
- remove(): 从集合中删除一个由next()方法返回的元素。

```java
Iterator<String>iterator=list.listIterator();
		while (iterator.hasNext()) {

			System.out.println(iterator.next());

			iterator.remove();//移除list中的元素
		}
 ```
如果集合中的元素没有排序，Iterator遍历集合中的元素顺序是任意的，并不一定与向集合中加入元素的顺序一致。
不同线程中对对Collection中的元素进行修改，在调用.next()方法时会报```
ConcurrentModificaionException ```异常。并发错误

----------
## ListIterator接口 ##
继承自Iterator接口，此外还提供了一些专门操纵List的一些方法
- add()向列表中加入一个元素,插入到next()或previous()元素之前
- hasPrevious()判断列表中是否还有上一个元素
- previous()返回列表中的上一个元素
 
## Set ##
### HashSet类 ###
当向集合中加入一个新的对象时，HashSet会调用对象的hashCode()产生哈希码，然后根据这个哈希码进一步算出对象的存储位置。
用对象的```equals()```方法和```hashCode()```（都是Object的方法）方法配合检查两个对象是否相等。如果用户覆盖了equals()方法则hashCode()方法也要一起覆盖。
```java
Set<String>set=new HashSet<String>();3
		set.add("姚腾辉1");
		set.add("姚腾辉2");
		Iterator<String>it=set.iterator();
		while(it.hasNext()){
			System.out.println(it.next());
		}
```
### TreeSet类 ###
TreeSet实现了SortSet接口，能够对集合中的对象进行排序
- 自然排序 插入对象时，调用对象的```compareTo()```方法进行比较，然后进行升序排序。
对象必须实现了Comparable接口（Integer,Double,String等）
TreeSet中只能加入同类型的对象。
- 客户化排序 （自定义排序，按照对象的某个属性 升序或降序排序）
 
```java
import java.util.Comparator;
import java.util.Iterator;
import java.util.Set;
import java.util.TreeSet;
//实现Comparator接口，在类中重写compare方法,设置要比较的属性和升/降序
public class CustomerSort implements Comparator<Customer>{
	@Override
	public int compare(Customer o1, Customer o2) {
		if (o1.getName().compareTo(o2.getName())>0) {return -1;
		}if (o1.getName().compareTo(o2.getName())<0) {return 1;			
		}
		return 0;
	}
	public static void main(String[]args){
		Customer c1=new Customer("yth1","22");
		Customer c2=new Customer("yth2","22");
		Customer c3=new Customer("yth3","22");
		Customer c4=new Customer("yth4","22");
//注意new TreeSet中的参数
		Set<Customer>set=new TreeSet<Customer>(new CustomerSort());
		set.add(c2);
		set.add(c3);
		set.add(c3);
		set.add(c4);
		TestMap tm=new TestMap();
		Iterator<Customer>it=set.iterator();
		while(it.hasNext()){
			System.out.println(it.next());
		}
	}
}
```
## Map ##
遍历Map中所有的键和值
```java
//Map.Entry代表Map中的一对键与值,map.entrySet()返回了一个Set集合，集合的元素是Map.Entry类型
for (Map.Entry<String, String> entry : map.entrySet()) {
		 System.out.println("键：" + entry.getKey() + " 值：" + entry.getValue());
		 }
```
TreeMap实现了对键对象进行排序。