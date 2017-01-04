## HTML/CSS ##
### 元素分类 ###
块级元素`display:block`:
1. 每个元素都是从新的一行开始，并且其后的元素也另起一行。
2. 元素的高度，宽，行高，顶部，底部边距都可以设置。
3. 宽度默认父容器100%
内联元素`display:inline`:
1. 和其他元素都在一行上；
2. 元素的高度、宽度及顶部和底部边距不可设置；
3. 元素的宽度就是它包含的文字或图片的宽度，不可改变。
内联块状元素`display:inline-block`
1. 和其他元素都在一行上；
2. 元素的高度、宽度、行高以及顶和底边距都可设置。

层模型(layer)
**绝对定位**
如果想为元素设置层模型中的绝对定位，需要设置`position:absolute`(表示绝对定位)，这条语句的作用将元素从文档流中拖出来，然后使用`left`、`right`、`top`、`bottom`属性相对于其**最接近的一个具有定位属性的父包**含块进行绝对定位。如果不存在这样的包含块，则相对于body元素，即相对于浏览器窗口。
**相对定位**
相对定位完成的过程是首先按static(float)方式生成一个元素(并且元素像层一样浮动了起来)，然后**相对于以前**的位置移动，移动的方向和幅度由left、right、top、bottom属性确定，**偏移前的位置保留不动。(他后面的元素，是相对于他偏移之前的位置来布局的)** `position:relative`
**固定定位**
相对移动的坐标是**视图（屏幕内的网页窗口）本身**。由于视图本身是固定的，它不会随浏览器窗口的滚动条滚动而变化，除非你在屏幕中移动浏览器窗口的屏幕位置，或改变浏览器窗口的显示大小，因此固定定位的元素会始终位于浏览器窗口内视图的某个位置，不会受文档流动影响，这与background-attachment:fixed;属性功能相同。

## 水平居中 ##
行内元素水平居中，在父元素设置`text-align:center`
### 宽度固定的块级元素 ###
```css
        div {
            width: 300px;
            border: 2px dashed red;
            margin: 20px auto;
        }
```

### 宽度不固定的水平居中 ###

将里面元素改为`display:inline`,
然后外层设置`text-align:center`
```html
<div class="container">
    <div class="div1">div1</div>
    <div class="div1">div2</div>
    <div class="div1">div3</div>
</div>
```
```css
        div.container {
            text-align: center;
            border: 1px solid black;
        }

        div.div1{
            display: inline;
        }
```
### 宽度不固定的水平居中2 ###
方法三：通过给父元素设置 float，然后给父元素设置 position:relative 和 left:50%，子元素设置 position:relative 和 left: -50% 来实现水平居中。

```html
<div class="container2">
    <div class="div2">div1</div>
    <div class="div3">div2</div>
    <div class="div3">div2</div>
</div>
```

```css
  div.div2 {
     border: 1px solid darkgreen;
     display: inline;
     position: relative;
     left: -50%;
     }
```
原理:div设置inline之后，它的宽度是wrap_content
首先以屏幕宽度50%开始绘制父布局，
然后以父布局-50%绘制子元素。
（50%都是相对于父布局的）


## Table ##
两个属性，<tr>标签里面，colspan表示水平合并<td>
rowspan表示竖直合并单元格 
