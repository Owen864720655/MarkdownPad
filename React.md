# React #
基于React进行开发时所有的DOM构造都是通过虚拟DOM进行，每当数据变化时，React都会重新构建整个DOM树，然后React将当前整个DOM树和上一次的DOM树进行对比，得到DOM结构的区别，然后仅仅将需要变化的部分进行实际的浏览器DOM更新

## ReactDOM.render() ##
将模板转化为html语言
```jsx
ReactDOM.render(
  <h1>Hello, world!</h1>,
  document.getElementById('example')
);
```

JSX基本语法规则:
遇到 HTML 标签（以 < 开头），就用 HTML 规则解析；遇到代码块（以 { 开头），就用 JavaScript 规则解析。



ES6中定义一个组件
```jsx
//ES6
class Photo extends React.Component {
    render() {
        return (
            <Image source={this.props.source} />
        );
    }
}
```
给组件定义方法
从上面的例子里可以看到，给组件定义方法不再用 名字: function()的写法，而是直接用名字()，在方法的最后也不能有逗号了。

```jsx
//ES5 
var Photo = React.createClass({
    componentWillMount: function(){

    },
    render: function() {
        return (
            <Image source={this.props.source} />
        );
    },
});
//ES6
class Photo extends React.Component {
    componentWillMount() {

    }
    render() {
        return (
            <Image source={this.props.source} />
        );
    }
}
```

初始化SRATE
```jsx
//ES5 
var Video = React.createClass({
    getInitialState: function() {
        return {
            loopsRemaining: this.props.maxLoops,
        };
    },
})
//ES6
class Video extends React.Component {
    constructor(props){
        super(props);
        this.state = {
            loopsRemaining: this.props.maxLoops,
        };
    }
}
```

ES6新增了let命令，用来声明变量。它的用法类似于var，但是所声明的变量，只在let命令所在的代码块内有效。


设置状态
```jsx
void setState(
  function|object nextState,
  [function callback]
)
//对象类型的
setState({mykey: 'my new value'});
```

## 样式 ##
```jsx
//创建样式对象
const styles = StyleSheet.create({
    bigblue: {
        color: 'blue',
        fontWeight: 'bold',
        fontSize: 30,
    },
    red: {
        fontSize: 20,
        color: 'red',
        borderColor: 'blue',
        backgroundColor: 'blue'
    },
});

//应用样式

 <Text style={styles.red}>just red</Text>
 <Text style={styles.bigblue}>just bigblue</Text>
```

## 宽高 和 弹性（Flex）宽高 ##
React Native中的尺寸都是无单位的，表示的是与设备像素密度无关的逻辑像素点。
宽 :width
高 :height


flex:1 (跟android里面的weight属性一个意思)
> 组件能够撑满剩余空间的前提是其父容器的尺寸不为零。如果父容器既没有固定的width和height，也没有设定flex，则父容器的尺寸为零。其子组件如果使用了flex，也是无法显示的。

```jsx
//View默认是垂直的LinearLayout所以如果根view的高不设的话就撑不开，也就无法显示
 <View style={{width: 300,height:400}}>
	 <View style={{flex: 1, backgroundColor: 'powderblue'}} />
	 <View style={{flex: 2, backgroundColor: 'skyblue'}} />
	 <View style={{flex: 3, backgroundColor: 'steelblue'}} />
</View>
```


## 使用Flexbox布局 ##
```jsx
flexDirection//排列方向column、row默认是colum

justifyContent//主轴的排列方式。子元素是应该靠近主轴的起始端还是末尾段分布呢？亦或应该均匀分布
flex-start:沿主轴从父布局左上角开始
center:沿主轴居中
flex-end:沿主轴从结尾处开始
space-around:沿着 主轴均分 切居中
space-between:可以让子元素被平均分布,第一子元素在容器最左边，最后一个子元素在最右边

//alignItems同理 可以决定其子元素沿着次轴（与主轴垂直的轴，比如若主轴方向为row，则次轴方向为column）的排列方式
stretch :是android中的march-parent(注意:对应的宽/高不能指定);
```
>`justifyContent` `alignItems` 这两个属性结合使用达到Android 中的gravity属性。
> android:gravity="center"相当于 {justifyContent：'center',alignItems:'cneter'}其他类似

**alignSelf**属性可以重置父布局中alignItems
```jsx
alignSelf: auto | flex-start | flex-end | center | baseline | stretch
.flex-item {
  align-self: flex-end;
}
```

**flexWrap**属性 默认情况下，项目都排在一条线（又称"轴线"）上。flexWrap属性定义，如果一条轴线排不下，如何换行。
```jsx
.box{
  flex-wrap: nowrap | wrap | wrap-reverse;
}
```
- nowrap（默认）：不换行。
- wrap：换行，第一行在上方。
- wrap-reverse：换行，第一行在下方。