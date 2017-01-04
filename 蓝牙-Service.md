获取BluetoothAdapter

BluetoothAdapter是Android系统中所有蓝牙操作都需要的，它对应本地Android设备的蓝牙模块，在整个系统中BluetoothAdapter是单例的。当你获取到它的示例之后，就能进行相关的蓝牙操作了。

连接GATT Server，你需要调用BluetoothDevice的 connectGatt() 方法。此函数带三个参数：Context、autoConnect(boolean) 实测发现，用false连接比较好和 BluetoothGattCallback 对象


BluetoothGatt常规用到的几个操作示例:

connect() ：连接远程设备。

discoverServices() : 搜索连接设备所支持的service。

disconnect()：断开与远程设备的GATT连接。

close()：关闭GATTClient端。

readCharacteristic(characteristic) ：读取指定的characteristic。

setCharacteristicNotification(characteristic, enabled)：设置当指定characteristic值变化时，发出通知。

getServices() ：获取远程设备所支持的services。

 BLE分为三部分：Service，Characteristic，Descriptor。这三部分都用UUID作为唯一标识符。UUID为这种格式：0000ffe1-0000-1000-8000-00805f9b34fb。比如有3个Service，那么就有三个不同的UUID与Service对应。这些UUID都写在硬件里，我们通过BLE提供的API可以读取到。
一个BLE终端可以包含多个Service， 一个Service可以包含多个Characteristic，一个Characteristic包含一个value和多个Descriptor，一个Descriptor包含一个Value。

对于 BLE 应用而言，当设备上的某一特定“特性”发生改变的时候，得到通知。
`setCharacteristicNotification()` 
如果对于某个“特性”的“通知”的功能可用，那么当该“特性”在远程设备上发生改变时，`onCharacteristicChanged()`回调便会被触发。 可以在该函数中发送广播

notify 就是让设备 可以发送通知给你，也可以说上报值给你（发送命令给你）
一旦设备那边notify 数据给你，你会在回调里收到：
```java
       @Override
                public void onCharacteristicChanged(BluetoothGatt gatt,
                                BluetoothGattCharacteristic characteristic) {

                        super.onCharacteristicChanged(gatt, characteristic);
                       //收到设备notify值 （设备上报值），根据 characteristic.getUUID()来判断是谁发送值给你，根据 characteristic.getValue()来获取这个值
                }
```

点击设备->进行连接->连接成功->发现服务->相关操作

相关博客: http://www.cnblogs.com/cxk1995/p/5693979.html
http://www.eoeandroid.com/thread-563868-1-1.html?_dsign=843d16d6
http://m.blog.csdn.net/article/details?id=50504406



































## Service相关啊
应用组件(客户端)可以调用bindService()绑定到一个service．Android系统之后调用service的onBind()方法，它返回一个用来与service交互的IBinder。

绑定是异步的．bindService()会立即返回，它不会返回IBinder给客户端．要接收IBinder，客户端必须创建一个ServiceConnection的实例并传给bindService()．ServiceConnection包含一个回调方法，系统调用这个方法来传递要返回的IBinder．


所以从客户端绑定到一个service 必须实现ServiceConnection;
- `onServiceConnected()` 系统调用这个来传送在service的onBind()中返回的IBinder
- `OnServiceDisconnected()`Android系统在同service的连接意外丢失时调用这个．比如当service崩溃了或被强杀了．当客户端解除绑定时，这个方法不会被调用．


--------------------------------------------------------------
    
    BluetoothGatt作为中央来使用和处理数据；BluetoothGattCallback返回中央的状态和周边提供的数据。

1.先拿到 `BluetoothManager：bluetoothManager = (BluetoothManager) getSystemService(Context.BLUETOOTH_SERVICE);`
2.再拿到`BluetoothAdapt：btAdapter = bluetoothManager.getAdapter();`
3.开始扫描：`btAdapter.startLeScan( BluetoothAdapter.LeScanCallback);`
4.从LeScanCallback中得到`BluetoothDevice：public void onLeScan(BluetoothDevice device, int rssi, byte[] scanRecord) {.....}`
5.用BluetoothDevice得到`BluetoothGatt：gatt = device.connectGatt(this, true, gattCallback);`


-----------------------------------------------
1. 绑定连接服务
this.context.bindService(serviceIntent, serviceConnection,
                    Service.BIND_AUTO_CREATE);

2. 获取绑定的服务
```java
    /**
     * 连接服务
     */
    private ServiceConnection serviceConnection = new ServiceConnection()
    {

        @Override
        public void onServiceDisconnected(ComponentName name)
        {
           // 系统调用这个来传送在service的onBind()中返回的IBinder
            bleService = null;
            Log.i(App.TAG, " gatt is onServiceDisconnected");
        }
//Android系统在同service的连接意外丢失时调用这个．比如当service崩溃了或被强杀了．当客户端解除绑定时，这个方法不会被调用．
        @Override
        public void onServiceConnected(ComponentName name, IBinder service)
        {
            bleService = ((RFStarBLEService.LocalBinder) service).getService();
            bleService.initBluetoothDevice(device);
            Log.i(App.TAG, " gatt is init");
        }
    };
```


## 广播 ##
### 1、注册广播事件：注册方式有两种 ###
1. 一种是静态注册，就是在 AndroidManifest.xml文件中定义，注册的广播接收器必须要继承BroadcastReceiver类；
在AndroidManifest.xml中用标签生命注册，并在标签内用标签设置过滤器。
2. 另一种是动态注册，是在程序中使用 Context.registerReceiver注册，注册的广播接收器相当于一个匿名类。两种方式都需要IntentFIlter。
```java
IntentFilter intentFilter = new IntentFilter();
intentFilter.addAction(String);   //为BroadcastReceiver指定action，使之用于接收同action的广播
registerReceiver(BroadcastReceiver,intentFilter);
```

一般：在onStart中注册，onStop中取消unregisterReceiver 

## 2、发送广播事件  
通过Context.sendBroadcast来发送，由Intent来传递注册时用到的Action

## 3.接收广播事件 ##
当发送的广播被接收器监听到后，会调用它的onReceive()方法，并将包含消息的Intent对象传给它。onReceive中代码的执行时间不要超过5s，否则Android会弹出超时dialog。

## 4、两类BroadcastReceiver ##
1. 正常广播 用 Context.sendBroadcast()发送
 可以在同一时刻（逻辑上）被所有接收者接收到，消息传递的效率比较高。但缺点是接受者不能将处理结果传递给下一个接收者，并且无法终止Broadcast Intent的广播。
2. 有序广播 用 Context.sendOrderedBroadcast()发送
每次被发送到一个receiver。所谓有序，就是每个receiver执行后可以传播到下一个receiver，也可以完全中止传播不传播给其他receiver。 而receiver运行的顺序可以通过matched intent-filter 里面的android:priority来控制，当priority优先级相同的时候，Receiver以任意的顺序运行。


-------------------------------------------------------

# Service #
> 启动方式

1. started ：一旦开启，该服务就可以 无限期 地在后台运行，哪怕开启它的组件被销毁掉。
2. bound 绑定： 形式的服务是指一个应用组件通过调用 bindService() 方法与服务绑定。一个绑定的服务提供一个客户-服务端接口，以允许组件与服务交互，发送请求，获得结果，甚至执行进程间通信。一个绑定的服务只和与其绑定的组件同时运行。多个组件可以同时绑定到一个服务，但当全部接触绑定后，服务就被销毁。

> 回调方法

`onStartCommand()` 当其他组件，如 activity 请求服务启动时，系统会调用这个方法。一旦这个方法执行，服务就开始并且无限期的执行。如果实现这个方法，当这个服务完成任务后，需要你来调用 stopSelf() 或者 stopService() 停掉服务。 如果只想提供绑定，不需要自己实现这个方法。
    
`onBind()` 当有其他组件想通过 bindService() 方法绑定这个服务时系统就会调用此方法。在实现的方法里面，必须添加一个供客户端使用的接口通过返回一个 IBinder 来与服务通信，这个方法必须实现。当然不想允许绑定的话，返回 null 即可。

`onCreate()`服务第一次建立的时候会调用这个方法，执行一次性设置程序.第一次创建service的时候调用

`onDestroy()`销毁的时候调用

## 在 manifest 文件声明服务 ##
要使用服务就必须在 manifest 文件声明要用的所有服务，只用在 <application> 标签内添加子标签 <service> 即可。
```xml
      <service android:name=".ExampleService"
        android:enabled=["true" | "false"]
     		android:exported=["true" | "false"]
     		android:isolatedProcess=["true" | "false"]
     		android:label="string resource"
     		android:icon="drawable resource"
     		android:permission="string"
     		android:process="string" >
        ...
      </service>
```

- android:name 你所编写的**服务类的类名**，可填写完整名称，包名+类名，如 com.example.test.ServiceA ，也可以忽略包名，用 . 开头，如 .ServiceA ，因为在 manifest 文件开头会定义包名，它会自己引用。

一旦你发布应用，你就不能改这个名字(除非设置 android:exported="false" )，另外 name 没有默认值，必须定义。
- android:enabled **是否可以被系统实例化**，默认为 true
因为父标签 <application> 也有 enable 属性，所以必须两个都为默认值 true 的情况下服务才会被激活，否则不会激活。
- android:exported **其他应用能否访问该服务**，如果不能，则只有本应用或有相同用户ID的应用能访问。当然除了该属性也可以在下面 permission 中限制其他应用访问本服务。

这个默认值与服务是否包含意图过滤器 intent filters 有关。如果一个也没有则为 false
- android:isolatedProcess 设置 true 意味着，服务会在一个特殊的进程下运行，这个进程与系统其他进程分开且没有自己的权限。与其通信的唯一途径是通过服务的API(binding and starting)。
android:label 可以显示给**用户的服务名称**。如果没设置，就用 <application> 的 lable 。不管怎样，这个值是所有服务的意图过滤器的默认 lable 。定义尽量用对字符串资源的引用。
android:icon 
类似 label ，是图标，尽量用 drawable 资源的引用定义。
android:permission
是一个实体必须要运行或绑定一个服务的权限。如果没有权限， startService() ， bindService() 或 stopService() 方法将不执行， Intent 也不会传递到服务。

如果属性未设置，会由 <application> 权限设置情况应用到服务。如果两者都未设置，服务就不受权限保护。
android:process
服务运行所在的进程名。通常为默认为应用程序所在的进程，与包名同名。 <application> 元素的属性 process 可以设置不同的进程名，当然组件也可设置自己的进程覆盖应用的设置。

如果名称设置为冒号 ： 开头，一个对应用程序私有的新进程会在需要时和运行到这个进程时建立。如果名称为小写字母开头，服务会在一个相同名字的全局进程运行，如果有权限这样的话。这允许不同应用程序的组件可以分享一个进程，减少了资源的使用。

> onStartCommand() 的返回值

1. START_NOT_STICKY 系统不重新创建服务，除非有将要传递来的 intent 。这是最安全的选项，可以避免在不必要的时候运行服务。
2. START_STICKY  系统重新创建服务并且调用 onStartCommand() 方法这个适合播放器一类的服务，不需要执行命令，只需要独自运行，等待任务。
3. START_REDELIVER_INTENT  系统重新创建服务并且调用 onStartCommand() 方法，传递最后一次传递的 intent 。其余存在的需要传递的intent会按顺序传递进来。这适合像下载一样的服务，立即恢复，积极执行。


## PendingIntent作用 ##
延迟的intent，主要用来在某个事件完成后执行特定的Action。PendingIntent包含了Intent及Context，所以就算Intent所属程序结束，PendingIntent依然有效，可以在其他程序中使用。
主要用途 :系统通知栏
短信系统



## 动态绑定服务 ##

### 新建服务 ###
```java
public class MyService extends Service
{
    public MyService()
    {
    }
    //定义一个说明客户端与服务通信方式的接口
    private MyBinder myBinder = new MyBinder();

    @Override
    public IBinder onBind(Intent intent)
    {
        return myBinder;
    }
//service测试方法
    public void showLog() {
        Log.i("服务测试", "service->showLog()");
    }
    public class MyBinder extends Binder
    {
        public MyService getService()
        {
            return MyService.this;
        }
    }
}
```
### 在manifests中注册 ###
```xml
        <service
            android:name=".MyService"
            android:enabled="true"
            android:exported="true">
        </service>
```
### ServiceConnection ###
```java
    private ServiceConnection serviceConnection=new ServiceConnection()
    {
        @Override
        public void onServiceConnected(ComponentName name, IBinder service)
        {
            Log.i("yth", " 绑定成功");
            MyService.MyBinder binder = (MyService.MyBinder) service;
            MyService SB = binder.getService();
            SB.showLog();
        }

        @Override
        public void onServiceDisconnected(ComponentName name)
        {
            //意外中断
            Log.i("yth", " 意外中断");
        }
    };
```
### 通过动态绑定一个服务 ###
```java
 Intent a = new Intent(MainActivity.this, MyService.class);
                bindService(a, serviceConnection, BIND_AUTO_CREATE);
```

 
> 整个过程就是通过绑定服务，绑定成功后在`ServiceConnection`中获取所绑定service的实例，然后对service中的方法进行操作

![service声明周期](http://i.imgur.com/ND83zu7.png)


通过startService开启服务
```java
   Intent a = new Intent(MainActivity.this, MyService2.class);
   startService(a);
```
在service中重写 开启一个前台通知
```java
    @Override
    public int onStartCommand(Intent intent, int flags, int startId)
    {
        Log.i("sss","-------");
        Intent i = new Intent(this, MainActivity.class);
        PendingIntent pendingIntent = PendingIntent.getActivity(this, 0, i, 0);

        Notification notification = new Notification.Builder(this).setContentTitle("Title")
            .setContentText("Message")
            .setSmallIcon(R.mipmap.ic_launcher)
            .setContentIntent(pendingIntent)
            .build();
        startForeground(12346, notification);
        return super.onStartCommand(intent,flags,startId);
    }
```


## 广播结合服务 ##

主界面中广播接收器,接收到广播后根据intent携带的参数更新界面
```java
    /**
     * 更新界面tv
     */
    private class UpdateUIBroadcastReceiver extends BroadcastReceiver {
        @Override
        public void onReceive(Context context, Intent intent) {
            textView.setText(String.valueOf(intent.getExtras().getInt("count")));
        }

    }
```

注册广播/启动服务
```java
 IntentFilter filter = new IntentFilter();
                filter.addAction(ACTION_UPDATEUI);
                broadcastReceiver = new UpdateUIBroadcastReceiver();
                //注册广播
                registerReceiver(broadcastReceiver, filter);
                // 启动服务
                Intent intenti = new Intent(this, MyService2.class);
                startService(intenti);
```

Service代码
在onCreate后每1s钟发一次广播
```java
package com.yth.test;

import android.app.Notification;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Intent;
import android.os.IBinder;

import java.util.Timer;
import java.util.TimerTask;

public class MyService2 extends Service
{
    Timer timer;
    private TimerTask task;
    private int count;

    public MyService2()
    {
    }

    @Override
    public IBinder onBind(Intent intent)
    {
        throw null;
    }

    @Override
    public void onDestroy()
    {
        count = 0;
        super.onDestroy();
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId)
    {
        Intent i = new Intent(this, MainActivity.class);
        PendingIntent pendingIntent = PendingIntent.getActivity(this, 0, i, 0);

        Notification notification = new Notification.Builder(this).setContentTitle("Title")
                .setContentText("Message")
                .setSmallIcon(R.mipmap.ic_launcher)
                .setContentIntent(pendingIntent)
                .build();
        startForeground(12346, notification);
        return super.onStartCommand(intent, flags, startId);
    }

    @Override
    public void onCreate()
    {
        super.onCreate();
        final Intent intent = new Intent();
        //1s钟发一次广播
        intent.setAction(MainActivity.ACTION_UPDATEUI);
        timer = new Timer();
        task = new TimerTask()
        {
            @Override
            public void run()
            {
                intent.putExtra("count", ++count);
                sendBroadcast(intent);
            }
        };
        timer.schedule(task, 1000, 1000);
    }
}

```