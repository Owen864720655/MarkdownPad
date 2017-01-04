## 自定义EditText实现右侧图标 ##

自定义`MyEditText`继承`EditText`

```java
package com.pump.yth.mywidget;

import android.content.Context;
import android.graphics.drawable.Drawable;
import android.util.AttributeSet;
import android.view.MotionEvent;
import android.view.View;
import android.widget.EditText;
/**
 * Created by yth on 2016/6/16.
 * 重写EditText加入对右侧图标的点击事件
 */
public class MyEditText extends EditText {
    private onDrawableRightListener mRightListener;
    final int DRAWABLE_RIGHT = 2;
    public MyEditText(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public MyEditText(Context context, AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }
    /**
     * 绑定监听事件
     *
     * @param listener onDrawableRightListener
     */
    public void setDrawableRightListener(onDrawableRightListener listener) {
        this.mRightListener = listener;
    }
    /**
     * 监听回调接口
     */
    public interface onDrawableRightListener {
        public void onDrawableRightClick(View view);
    }
//判断点击位置是否是右侧图标，如果是执行相应的回调函数。
    @Override
    public boolean onTouchEvent(MotionEvent event) {
        switch (event.getAction()) {
            case MotionEvent.ACTION_UP:
                if (mRightListener != null) {
                    Drawable drawableRight = getCompoundDrawables()[DRAWABLE_RIGHT];
                    if (drawableRight != null && event.getRawX() >= (getRight() - drawableRight.getBounds().width())) {
                        mRightListener.onDrawableRightClick(this);
                    }
                }
                break;
        }
        return super.onTouchEvent(event);
    }
}
```

使用：
```java
        //右侧图标点击事件
        mPasswordView.setDrawableRightListener(new MyEditText.onDrawableRightListener() {

            @Override
            public void onDrawableRightClick(View view) {
                //密码可见状态取反
                IS_PASSWORD_SHOW = !IS_PASSWORD_SHOW;
                Drawable drawableright;
                if (IS_PASSWORD_SHOW) {
                    mPasswordView.setTransformationMethod(HideReturnsTransformationMethod.getInstance());
                    drawableright = getResources().getDrawable(R.drawable.ic_password_hidden);
                } else {
                    mPasswordView.setTransformationMethod(PasswordTransformationMethod.getInstance());
                    drawableright = getResources().getDrawable(R.drawable.ic_password_show);
                }
                mPasswordView.setCompoundDrawablesWithIntrinsicBounds(null, null, drawableright, null);
            }
        });
```
>注意：
>用到的一些知识点
>Android MotionEvent事件
>Android View坐标

### Android MotionEvent事件响应机制 ###

**事件**主要包括点击、长按、拖曳、滑动等操作，所有的事件都由如下三个部分作为基础构成：按下（action_down），移动（action_move），抬起（action_up）。
**响应**归根结底都是基于View以及ViewGroup的，这两者中响应的方法分别有：
在`View.java`中
```java
publi boolean dispatchTouchEvent(MotionEvent event)
public boolean onTouchEvent(MotionEvent event)
```
在ViewGroup.java中
```java
public boolean dispatchTouchEvent(MotionEvent event)
public boolean onTouchEvent(MotionEvent event) 
public boolean onInterceptTouchEvent(MotionEvent event)
```
>MotionEvent对象的理解：

当用户触摸屏幕时将创建一个MotionEvent对象。MotionEvent包含关于发生触摸的位置和时间等细节信息。MotionEvent对象 被传递到程序中合适的 方法比如View对象的onTouchEvent()方法中。在这些方法中我们可以分析MotionEvent对象，以决定要执行的操作。

MotionEvent对象是与用户触摸相关的时间序列，该序列从用户首次触摸屏幕开始，经历手指在屏幕表面的任何移动，直到手指离开屏幕时结束。手指的初次触摸(ACTION_DOWN操作)，滑动(ACTION_MOVE操作)和抬起(ACTION_UP)都会创建MotionEvent对象。所以每次触摸时候这三个操作是肯定发生的，而在移动过程中会产生大量事件，每个事件都会产生对应的MotionEvent对象记录发生的操作，触摸的位置，使用的多大压力，触摸的面积，合适发生，以及最初的ACTION_DOWN和时发生等相关的信息。
//----------------------------
`dispatchTouchEvent`此函数负责事件的分发，当触摸一个View控件，首先会调用这个函数就行，在这个函数体里决定将事件分发给谁来处理。它拥有boolean类型的返回值，当返回为true时，顺序下发会中断。

`onTouchEvent` 此函数负责执行事件的处理，负责处理事件

`onInterceptTouchEvent`主要来决定当前控件是否需要拦截传递给子控件，如果返回True表示该控件拦截，并交由自己父类的dispatchTouchEvent处理消费，如果返回false表示不拦截，允许传递给子控件处理。

当Acitivty接收到Touch事件时，将遍历子View进行Down事件的分发。ViewGroup的遍历可以看成是递归的。分发的目的是为了找到真正要处理本次完整触摸事件的View，这个View会在onTouchuEvent结果返回true。
当某个子View返回true时，会中止Down事件的分发，同时在ViewGroup中记录该子View。接下去的Move和Up事件将由该子View直接进行处理。

当ViewGroup中所有子View都不捕获Down事件时，将触发ViewGroup自身的onTouch事件。触发的方式是调用super.dispatchTouchEvent函数，即父类View的dispatchTouchEvent方法。在所有子View都不处理的情况下，触发Acitivity的onTouchEvent方法。

onInterceptTouchEvent有两个作用：1.拦截Down事件的分发。2.中止Up和Move事件向目标View传递，使得目标View所在的ViewGroup捕获Up和Move事件。

//---------------------------------------------------------------------------
整个View的事件转发流程是:
View.dispatchEvent->View.setOnTouchListener->View.onTouchEvent

思路:返回ture事件被消耗，后面的不执行了。

View.dispatchEvent->View.setOnTouchListener（onTouch）->onTouchEvent->onClick。当onTouch返回false时，onTouchEvent才会执行。当onTouchEvent显式调用onClick时(一个完整的触发，down,up)，onClick才会执行。

在dispatchTouchEvent中会进行OnTouchListener的判断，如果OnTouchListener不为null且返回true，则表示事件被消费，onTouchEvent不会被执行；否则执行onTouchEvent。
![View事件分发](http://i.imgur.com/UWMF5D4.png).

参考:
http://www.cnblogs.com/linjzong/p/4191891.html
http://www.2cto.com/kf/201406/308037.html
http://blog.csdn.net/lmj623565791/article/details/38960443
http://www.android100.org/html/201502/26/123262.html 做实验验证一样。


`getLeft()`和`getTop()`可以得到一个View的位置。这两个方法返回的是相对于其父元素的位置
```java
getRight() =getLeft() + getWidth()
getBottom()= getTop() + getHeight()

```
```java
//可以在上、下、左、右设置图标，如果不想在某个地方显示，则设置为null。图标的宽高将会设置为固有宽高，既自动通过getIntrinsicWidth和getIntrinsicHeight获取
setCompoundDrawablesWithIntrinsicBounds(Drawable left, Drawable top, Drawable right, Drawable bottom)
//可以在上、下、左、右设置图标，如果不想在某个地方显示，则设置为null。但是Drawable必须已经setBounds(Rect)
setCompoundDrawables(Drawable left, Drawable top, Drawable right, Drawable bottom)
//这个方法要先给Drawable设置
setBounds(x,y,width,height);
```
```java
//设置EdiText的password的隐藏与显示
setTransformationMethod
```