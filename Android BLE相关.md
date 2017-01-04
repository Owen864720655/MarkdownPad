## Generic Attribute Profile (GATT)
通过BLE连接，读写属性类小数据的Profile通用规范。现在所有的BLE应用Profile都是基于GATT的。

## Attribute Protocol (ATT)
GATT是基于ATT Protocol的。ATT针对BLE设备做了专门的优化，具体就是在传输过程中使用尽量少的数据。
每个属性都有一个唯一的UUID，属性将以characteristics and services的形式传输。

## Characteristic ##
Characteristic可以理解为一个数据类型，它包括一个value和0至多个对次value的描述（Descriptor）。

## Descriptor ##
对Characteristic的描述，例如范围、计量单位等。

## Service ##
Characteristic的集合。例如一个service叫做“Heart Rate Monitor”，
它可能包含多个Characteristics，其中可能包含一个叫做“heart rate measurement"的Characteristic。