##按键输入实验##
1. 使能GPIO时钟
2. 初始化GPIO

##串口操作##
串口设置的一般步骤可以总结为如下几个步骤： 
1) 串口时钟使能，GPIO 时钟使能 
RCC_APB2PeriphClockCmd(RCC_APB2Periph_USART1)；
2) 串口复位 
void USART_DeInit(USART_TypeDef* USARTx);
3) GPIO 端口模式设置 
4) 串口参数初始化 
void USART_Init(USART_TypeDef* USARTx, USART_InitTypeDef* USART_InitStruct)；
5) 开启中断并且初始化 NVIC（如果需要开启中断才需要这个步骤） 
void USART_ITConfig(USART_TypeDef* USARTx, uint16_t USART_IT, FunctionalState NewState) 
6) 使能串口 
USART_Cmd(USART1, ENABLE); 
7) 编写中断处理函数 



## 串口相关 ##
### 数据发送与接收 ###
STM32 的发送与接收是通过数据寄存器 **USART_DR** 来实现的，这是一个双寄存器，包含了 TDR 和 RDR。当向该寄存器写数据的时候，串口就会自动发送，当收到数据的时候，也是存在该寄存器内。
 
写入：
void USART_SendData(USART_TypeDef* USARTx, uint16_t Data); 

读出：
uint16_t USART_ReceiveData(USART_TypeDef* USARTx); 

### 串口状态 ###
串口的状态可以通过状态寄存器 **USART_SR** 读取.第 5、6 位 RXNE 和 TC。

**RXNE**（读数据寄存器非空），当该位被置 1 的时候，就是提示已经有数据被接收到了，并且可以读出来了。这时候我们要做的就是尽快去读取 USART_DR，通过读 USART_DR 可以将该位清零，也可以向该位写 0，直接清除。
 
**TC**（发送完成），当该位被置位的时候，表示 USART_DR 内的数据已经被发送完成了。如果设置了这个位的中断，则会产生中断。该位也有两种清零方式：1）读 USART_SR，写
USART_DR。2）直接向该位写 0


读取串口状态的函数是： 
FlagStatus USART_GetFlagStatus(USART_TypeDef* USARTx, uint16_t USART_FLAG)；

开启串口响应中断:
void USART_ITConfig(USART_TypeDef* USARTx, uint16_t USART_IT, FunctionalState NewState) 

获取相应中断状态:
ITStatus USART_GetITStatus(USART_TypeDef* USARTx, uint16_t USART_IT) 

----------
流程
1. 使能USART GPIOA时钟
2. 复位串口1
3. 初始化GPIOA 的PA.9和PA.10
4. USART初始化
5. USART中断配置
6. 开启中断
7. 使能串口
8. 如果接受到了中断 将接受的数据存储起来.(存储的策略：状态机来做，先分析有几个状态，状态之间的转换。，在转换过程中 存储，并且设置状态)
## 看门狗 ##
一个装满时间数值的倒计时器一样，启动之后就开始倒计时，倒计时为零传送一个复位信号到 CUP，又重新开始倒计时。 
独立的看门狗是基于一个 12 位的递减计数器和一个 8 位的预分频器
### 相关的寄存器 ###

（Key Register）键值寄存器 IWDG_KR:用低16位
0xCCCC//开启独立开门狗
0xAAAA//避免产生复位
0x5555//去除IWDG_PR 和 IWDG_RLR写保护功能

（Prescaler Register）预分频器IWDG_PR 用低三位
（Reload Register）重装载寄存器 IWDG_RLR用低12位。

看门狗复位时间=(预分频×重装载值) /40KHz 

库函数操作流程：

1. 取消寄存器写保护
	 IWDG_WriteAccessCmd(IWDG_WriteAccess_Enable); 
2. 设置独立看门狗的预分频系数和重装载值 
     void IWDG_SetPrescaler(uint8_t IWDG_Prescaler); //设置 IWDG 预分频值
	 void IWDG_SetReload(uint16_t Reload); //设置 IWDG 重装载值
3. 启动看门狗
	 IWDG_Enable(); //使能 IWDG
4. 重载计数值喂狗
    IWDG_ReloadCounter(); //按照 IWDG 重装载寄存器的值重装载 IWDG 计数器

## 窗口看门狗 ##
除非递减计数器的值在 T6 位变成 0 前被刷新，看门狗电路在达到预置的时间周期时，会产生一个 MCU复位。


T[6:0]就是 **WWDG_CR 的低七位**，W[6:0]即是 **WWDG->CFR 的低七位**。T[6:0]
就是窗口看门狗的计数器，而 W[6:0]则是窗口看门狗的上窗口，下窗口值是固定的（0X40）。
当窗口看门狗的计数器在上窗口值之外被刷新，或者低于下窗口值都会产生复位。 
上窗口值（W[6:0]）是由用户自己设定的，根据实际要求来设计窗口值，但是一定要确保
窗口值大于 0X40，否则窗口就不存在了。
WDGA:激活位 0：禁止看门狗 1：启用看门狗 
相关寄存器：
**控制寄存器（WWDG_CR）**T[6：0]用来存储看门狗的计数器值。
**WDGA** 位则是看门狗的激活位，该位由软件置 1，以启动看门狗，并且一定要注意的是该
位一旦设置，就只能在硬件复位后才能清零了。 
**配置寄存器（WWDG_CFR）**
EWI(9位) 是提前唤醒中断W[6:0]则是窗口看门狗的上窗口
WDGTB(8:7)时基
**状态寄存器（WWDG_SR）**
记录当前是否有提前唤醒的标志。该寄存器仅有位 0 有效
## STM32中断记录 ##
STM32 的中断具有两个属性，一个为抢先属性，另一个为响应属性，**其属性编号越小，表明它
的优先级别越高。**
抢占优先级是NVIC_IRQChannelPreemptionPriority配置的
响应优先级是在抢占优先级相同的情况下 同时中断时触发。由NVIC_IRQChannelSubPriority配置。
### 中断操作 ###
中断分组函数 NVIC_PriorityGroupConfig( )
NVIC 初始化函数 NVIC_Init() 
```c
NVIC_IRQChannel 需要配置的中断向量 
NVIC_IRQChannelCmd 使能或关闭相应中断向量的中断响应 
NVIC_IRQChannelPreemptionPriority 配置中断向量抢先优先级 
NVIC_IRQChannelSubPriority 配置中断向量的响应优先级 
```
##  STM32 外部中断 ##
**EXTI**: External Interrupts 外部中断
一共有EXTI0-EXTI1516组。
GPIO中断分为类为AIFO_EXTIx的低四位。
所以每个EXTIx里面可以有8个GPIO口。
相关配置
1. 配置GPIO与中断线的映射关系
  void GPIO_EXTILineConfig(uint8_t GPIO_PortSource, uint8_t GPIO_PinSource)
2. 中断线上中断的初始化
 void EXTI_Init(EXTI_InitTypeDef* EXTI_InitStruct);
```c
	EXTI_InitTypeDef EXTI_InitStructure; 
 	EXTI_InitStructure.EXTI_Line=EXTI_Line4; //中断线的标号EXTI_Line0-EXTI_Line15
 	EXTI_InitStructure.EXTI_Mode = EXTI_Mode_Interrupt; //中断模式
 	EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Falling; //触发方式
 	EXTI_InitStructure.EXTI_LineCmd = ENABLE; //使能中断线
 	EXTI_Init(&EXTI_InitStructure); //根据 EXTI_InitStruct 中指定的 
```
3. 中断服务函数
```c
EXPORT EXTI0_IRQHandler 
EXPORT EXTI1_IRQHandler 
EXPORT EXTI2_IRQHandler 
EXPORT EXTI3_IRQHandler 
EXPORT EXTI4_IRQHandler 
EXPORT EXTI9_5_IRQHandler 
EXPORT EXTI15_10_IRQHandler 
```
判断某个中断线上的中断是否发生:
ITStatus EXTI_GetITStatus(uint32_t EXTI_Line)； 
清除某个中断线上的中断标志位:
void EXTI_ClearITPendingBit(uint32_t EXTI_Line)；
```c
void EXTI2_IRQHandler(void) 
{ 
if(EXTI_GetITStatus(EXTI_Line3)!=RESET)//判断某个线上的中断是否发生
 { 
 中断逻辑… 
 EXTI_ClearITPendingBit(EXTI_Line3); //清除 LINE 上的中断标志位
 } 
} 
```

IO 口外部中断的一般步骤： 
1. 初始化 IO 口为输入。 
2. 开启 IO 口复用时钟，设置 IO 口与中断线的映射关系。 
3. 初始化线上中断，设置触发条件等。 
4. 配置中断分组（NVIC），并使能中断。 
5. 编写中断服务函数。 

## 定时器中断 ##
1. 时钟使能
RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM3, ENABLE); //时钟使能
2. 初始化定时器参数,设置自动重装值，分频系数，计数方式等
voidTIM_TimeBaseInit(TIM_TypeDef*TIMx, TIM_TimeBaseInitTypeDef*TIM_TimeBaseInitStruct); 
	```c
	TIM_TimeBaseInitTypeDef TIM_TimeBaseStructure; 
	
	TIM_TimeBaseStructure.TIM_Prescaler =7199; //分频系数
	TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;//计数模式
	TIM_TimeBaseStructure.TIM_Period = 5000; //自动重载计数周期值
	TIM_TimeBaseStructure.TIM_ClockDivision = TIM_CKD_DIV1; //时钟分频因子
	
	TIM_TimeBaseStructure.TIM_RepetitionCounter;//高级定时器需要
	
	TIM_TimeBaseInit(TIM3, &TIM_TimeBaseStructure); 
	```
3. 允许TIMx_DIER更新中断
void TIM_ITConfig(TIM_TypeDef* TIMx, uint16_t TIM_IT, FunctionalState NewState)；
第一个参数是选择定时器号，这个容易理解，取值为 TIM1~TIM17。
第二个参数用来指明我们使能的定时器中断的类型，定时器中断的类型有很
多种，包括更新中断 TIM_IT_Update，触发中断 TIM_IT_Trigger，以及输入捕获中断等等
第三个参数 使能
4. TIM3 中断优先级设置NVIC_Init（）
5. 使能 TIMx
	void TIM_Cmd(TIM_TypeDef* TIMx, FunctionalState NewState) 
6. 编写中断服务函数。
	判断中断类型 ITStatus TIM_GetITStatus(TIM_TypeDef* TIMx, uint16_t) 
	清除中断标志位 void TIM_ClearITPendingBit(TIM_TypeDef* TIMx, uint16_t TIM_IT)
	```c
	 if (TIM_GetITStatus(TIM3, TIM_IT_Update) != RESET) //检查 TIM3 更新中断发生与否 
	 { 
	 TIM_ClearITPendingBit(TIM3, TIM_IT_Update ); //清除 TIM3 更新中断标志 
	 //中断逻辑 
	 } 
	```

## PWM ##
可以产生一个由TIMx_ARR寄存器确定的频率，由TIMx_CCRx寄存器确定占空比的信号。

捕获/比较模式寄存器（TIMx_CCMR1/2）OCxM这三位来设置模式，共7种模式
捕获/比较使能寄存器（TIMx_CCER)
捕获/比较寄存器（TIMx_CCR1~4)

步骤：
1. 开启 TIM3 时钟以及复用功能时钟，配置 PB5 为复用输出。 
需要用到端口复用和重映射。PB5复用为PWM输出。重映射将TIM3_CH2 通道重映射到 PB5 
RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM3, ENABLE);	//使能定时器3时钟
 	RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB  | RCC_APB2Periph_AFIO, ENABLE);  //使能GPIO外设和AFIO复用功能模块时钟
2.  设置部分重映射
 GPIO_PinRemapConfig(GPIO_PartialRemap_TIM3, ENABLE);//TIM3_CH2->PB5部分重映射
3. 初始化TIM3,设置TIM3的ARR和PSC。
	```c
    //初始化TIM3
	TIM_TimeBaseStructure.TIM_Period = arr; //设置在下一个更新事件装入活动的自动重装载寄存器周期的值
	TIM_TimeBaseStructure.TIM_Prescaler =psc; //设置用来作为TIMx时钟频率除数的预分频值 
	TIM_TimeBaseStructure.TIM_ClockDivision = 0; //设置时钟分割:TDTS = Tck_tim
	TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;  //TIM向上计数模式
	TIM_TimeBaseInit(TIM3, &TIM_TimeBaseStructure); //根据TIM_TimeBaseInitStruct中指定的参数初始化TIMx的时间基数单位
    ```
4. 设置 TIM3_CH2 的 PWM 模式，使能 TIM3 的 CH2 输出
	PWM 通道设置是通过函数 TIM_OC1Init()~TIM_OC4Init()来设置的
	```c
	//初始化TIM3 Channel2 PWM模式	 
	TIM_OCInitStructure.TIM_OCMode = TIM_OCMode_PWM2; //选择定时器模式:TIM脉冲宽度调制模式2
 	TIM_OCInitStructure.TIM_OutputState = TIM_OutputState_Enable; //比较输出使能使能 也就是PWM 输出到端口
	TIM_OCInitStructure.TIM_OCPolarity = TIM_OCPolarity_High; //输出极性:TIM输出比较极性高
	//还有一些参数在高级定时器才会用。

	TIM_OC2Init(TIM3, &TIM_OCInitStructure);  //根据T指定的参数初始化外设TIM3 OC2
	
	```
5. TIM_OC2PreloadConfig(TIM3, TIM_OCPreload_Enable); //使能TIM3在CCR2上的预装载寄存器,即TIM3_CCR2的预装载值在更新事件到来时才能被传送至当前寄存器中。

6. 使能TIM3
	TIM_Cmd(TIM3, ENABLE); //使能 TIM3

7.  TIM3_CCR2 来控制占空比
	void TIM_SetCompare2(TIM_TypeDef* TIMx, uint16_t Compare2)； 



## 输入捕获实验 ##

1. 使能TIM5时钟和GPIOA时钟
```c
RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM5, ENABLE);	//使能TIM5时钟
RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOA, ENABLE);  //使能GPIOA时钟
```
2. 初始化TIM5
3. 初始化TIM5的输入捕获参数
```c
TIM_ICInitTypeDef TIM5_ICInitStructure; 
TIM5_ICInitStructure.TIM_Channel = TIM_Channel_1; //选择输入端 IC1 映射到 TI1
TIM5_ICInitStructure.TIM_ICPolarity = TIM_ICPolarity_Rising; //上升沿捕获 
TIM5_ICInitStructure.TIM_ICSelection = TIM_ICSelection_DirectTI; //直接映射到 TI1 上
TIM5_ICInitStructure.TIM_ICPrescaler = TIM_ICPSC_DIV1; //配置输入分频,不分频
TIM5_ICInitStructure.TIM_ICFilter = 0x00;//IC1F=0000 配置输入滤波器 不滤波 
TIM_ICInit(TIM5, &TIM5_ICInitStructure); 
```
4. 使能中断
```c
TIM_ITConfig(TIM5,TIM_IT_Update|TIM_IT_CC1,ENABLE);//允许更新中断，允许CC1IE捕获中断
```
5. 中断分组初始化 编写中断服务函数
```c
	NVIC_InitStructure.NVIC_IRQChannel = TIM5_IRQn;  //TIM5初始化
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 2;  //抢占优先级2
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;  //从优先级0
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE; //IRQÍ通道被使能
	NVIC_Init(&NVIC_InitStructure);  
```
```c
if (TIM_GetITStatus(TIM5, TIM_IT_Update) != RESET){}//判断是否为更新中断 
if (TIM_GetITStatus(TIM5, TIM_IT_CC1) != RESET){}//判断是否发生捕获事件 
TIM_ClearITPendingBit(TIM5, TIM_IT_CC1|TIM_IT_Update);//清除中断和捕获标志位
```
6. 使能定时器
```c
TIM_Cmd(TIM5,ENABLE ); //使能定时器 5 
```
7. 编写中断服务函数
```c
void TIM5_IRQHandler(void)
{ 
	if (TIM_GetITStatus(TIM5, TIM_IT_Update) != RESET)
	//捕获更新中断
	{
	
	}
	if (TIM_GetITStatus(TIM5, TIM_IT_CC1) != RESET)//捕获1发生捕获事件
	{

	}
}
```


----------

## RTC实时时钟 ##
RTC(Reak-Time-Clock)实时时钟，即使开发板掉电再上电开发板的时间不会发生错误。

1. 使能电源时钟和备份域时钟
	```c
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_PWR | RCC_APB1Periph_BKP, ENABLE);
	```
2. 取消备份区写保护(写保护在每次硬复位之后被使能)

	```c
	PWR_BackupAccessCmd(ENABLE); //使能 RTC 和后备寄存器访问 
	```
3. 复位备份区域，开启外部低速振荡器 ( 低速振荡器已经就绪之后再进行操作)
	```c
	BKP_DeInit();//复位备份区域 (看情况而定)
	RCC_LSEConfig(RCC_LSE_ON);// 开启外部低速振荡器
	```
4. 选择 RTC 时钟，并使能。
	```c
	RCC_RTCCLKConfig(RCC_RTCCLKSource_LSE); //选择 LSE 作为 RTC 时钟
	RCC_RTCCLKCmd(ENABLE); //使能 RTC 时钟 
	```
5. 设置 RTC 的分频，以及配置 RTC 时钟
	1. 等待 RTC 寄存器操作完成，并同步之后，设置秒钟中断
	2. 设置RTC 的允许配置位（RTC_CRH 的 CNF 位）
	3. 设置 RTC 时钟的分频数
	4. 设置时间（ RTC_CNTH 和 RTC_CNTL两个寄存器）
	```c
	RTC_WaitForLastTask();	//等待最近一次对RTC寄存器的写操作完成
	RTC_WaitForSynchro();	//等待RTC寄存器同步
	RTC_ITConfig(RTC_IT_SEC, ENABLE);	//使能秒中断
	
	RTC_WaitForLastTask();	//等待最近一次对RTC寄存器的写操作完成
	RTC_EnterConfigMode();//允许配置	
	RTC_SetPrescaler(32767); //设置预分频值
	RTC_WaitForLastTask();	//等待最近一次对RTC寄存器的写操作完成		
	RTC_Set(2009,12,2,10,0,55);  //设置时间
	RTC_ExitConfigMode(); //退出配置模式
	```


![](http://i.imgur.com/qdq5nLH.jpg)
注意：
对RTC_PRL, RTC_CNT, RTC_ALR写入之前需要进入配置模式，写入之后退出配置模式.
对RTC寄存器写入之前都需要判断RTOFF位是否为1 。该位指示最后一次操作的状态。
疑问：
	RTC寄存器什么时候需要被同步? 复位之后吗？


## ADC数模转换实验 ##
**逐次逼近转换**
转换开始前先将所有寄存器清零。开始转换以后 存器最高位置成 1，使输出数字为 100…0。这个数被数模转换器转换成相应的 拟电压 Uo，送到比较器中与 Ui 进行比较。若 Uo＞Ui，说明数字过大了，故将最高位的 1 清除；
若 Uo＜Ui ，说明数字还不够大，应将最高位的 1 保留。然后，再按同样的方式将次高位置成 1，并且经过比较以后确定这个 1 是否应该保留。这样逐位比较下去，一直到最低位为止。比较完毕后，寄存器中的状态就是所要求的数字量输出。 


STM32将ADC的转换分为2个通道组：规则通道组和注入通道组。


实验步骤：

1. 开启 PA 口时钟和 ADC1 时钟，设置 PA1 为模拟输入。
```c
RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOA |RCC_APB2Periph_ADC1, ENABLE );	  //使能ADC1通道时钟
RCC_ADCCLKConfig(RCC_PCLK2_Div6);   //设置ADC分频因子6 72M/6=12,ADC最大时间不能超过14M
GPIO_InitStructure.GPIO_Pin = GPIO_Pin_1;//PA1 作为模拟通道输入引脚 
GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AIN;//模拟输入引脚
GPIO_Init(GPIOA, &GPIO_InitStructure);	
```
2. 复位 ADC1，同时设置 ADC1 分频因子。

```c
RCC_ADCCLKConfig(RCC_PCLK2_Div6);   //设置ADC分频因子6 72M/6=12,ADC最大时间不能超过14M
ADC_DeInit(ADC1);  //复位ADC1,将外设 ADC1 的全部寄存器重设为缺省值  要在初始化ADC参数之前
```
3.初始化 ADC1 参数
```c

    ADC_InitStructure.ADC_Mode = ADC_Mode_Independent;	//ADC工作模式:ADC1和ADC2工作在独立模式

	ADC_InitStructure.ADC_ScanConvMode = DISABLE;	//模数转换工作在单通道模式

	ADC_InitStructure.ADC_ContinuousConvMode = DISABLE;	//模数转换工作在单次转换模式

	ADC_InitStructure.ADC_ExternalTrigConv = ADC_ExternalTrigConv_None;	//转换由软件而不是外部触发启动

	ADC_InitStructure.ADC_DataAlign = ADC_DataAlign_Right;	//ADC数据右对齐

	ADC_InitStructure.ADC_NbrOfChannel = 1;	//顺序进行规则转换的ADC通道的数目

	ADC_Init(ADC1, &ADC_InitStructure);	//根据ADC_InitStruct中指定的参数初始化外设ADCx的寄存器   

```
说明：
ADC_Mode  STM32的ADC模式有同步注入模式、同步规则模式等10种

ADC_ScanConvMode 当有多个通道需要采集信号时，可以开启此模式，把ADC配置为轮流采集各通道的值。

ADC_ContinuousConvMode 连续转换模式 单次转换模式ADC只采集一次数据就停止转换。而
连续转换模式则在上一次ADC转换完成后，立即开启下一次转换。

ADC_ExternalTrigConv  ADC需要进行触发才开始进行模数转换，触发的方式可以是接收外部触发信号进行触发，也可以是使用软件控制触发。外部触发信号有外部中断触发(EXTI线)、定时器触发。
 
ADC_DataAlign 数据对齐方式，有左对齐、右对齐两种(转换的数值是12位，存在16位寄存器中)

ADC_NbrOfChannel 将被转换的ADC通道的数量

4. 使能 ADC 并校准
```c
    ADC_Cmd(ADC1, ENABLE);	//使能指定的ADC1

	ADC_ResetCalibration(ADC1);	//使能复位校准  
	 
	while(ADC_GetResetCalibrationStatus(ADC1));	//等待复位校准结束
	
	ADC_StartCalibration(ADC1);	 //开启ADC校准
 
	while(ADC_GetCalibrationStatus(ADC1));	 //等待校准结束
```
5. 读取 ADC 值
```c
//获得ADC值
//ch:通道值 0~3
u16 Get_Adc(u8 ch)   
{
  	//设置指定ADC的规则组通道，一个序列，采样时间
	ADC_RegularChannelConfig(ADC1, ch, 1, ADC_SampleTime_239Cycles5 );	//配置采样周期	  			    
  
	ADC_SoftwareStartConvCmd(ADC1, ENABLE);		//使能指定的ADC1的软件转换启动功能	
	 
	while(!ADC_GetFlagStatus(ADC1, ADC_FLAG_EOC ));//等待转换结束

	return ADC_GetConversionValue(ADC1);	//返回最近一次ADC1规则组的转换结果
}
```

----------


## DMA ##
主要功能是通信“桥梁”的作用，可以将所有外设映射的寄存器“连接”起来，这样就可以高速问各寄存器
1. 使能时钟 
```c
 RCC_AHBPeriphClockCmd(RCC_AHBPeriph_DMA1, ENABLE);	//使能DMA传输
	
   DMA_DeInit(DMA_CHx);
```
2. DMA初始化

```c
  	DMA_DeInit(DMA_CHx);   //将DMA的通道x寄存器重设为缺省值 ,恢复默认值
	DMA1_MEM_LEN=cndtr;


	DMA_InitStructure.DMA_PeripheralBaseAddr = cpar;  //DMA外设ADC基地址
	DMA_InitStructure.DMA_MemoryBaseAddr = cmar;  //DMA内存基地址
	DMA_InitStructure.DMA_DIR = DMA_DIR_PeripheralDST;  //数据传输方向，从内存读取发送到外设
	DMA_InitStructure.DMA_BufferSize = cndtr;  //DMA通道的DMA缓存的大小
	DMA_InitStructure.DMA_PeripheralInc = DMA_PeripheralInc_Disable;  //外设地址寄存器不变
	DMA_InitStructure.DMA_MemoryInc = DMA_MemoryInc_Enable;  //内存地址寄存器递增
	DMA_InitStructure.DMA_PeripheralDataSize = DMA_PeripheralDataSize_Byte;  //数据宽度为8位
	DMA_InitStructure.DMA_MemoryDataSize = DMA_MemoryDataSize_Byte; //数据宽度为8位
	DMA_InitStructure.DMA_Mode = DMA_Mode_Normal;  //工作在正常缓存模式
	DMA_InitStructure.DMA_Priority = DMA_Priority_Medium; //DMA通道 x拥有中优先级 
	DMA_InitStructure.DMA_M2M = DMA_M2M_Disable;  //DMA通道x没有设置为内存到内存传输
	DMA_Init(DMA_CHx, &DMA_InitStructure);  //根据DMA_InitStruct中指定的参数初始化DMA的通道USART1_Tx_DMA_Channel所标识的寄存器

```
一些值的解释：
DMA_DIR  设置数据传输方向，决定是从外设读取数据到内存还送从内存读取数据发送到外设。
DMA_BufferSize 设置一次传输数据量的大小。
DMA_PeripheralInc 设置传输数据的时候**外设地址**是不变还是递增。
DMA_MemoryInc 设置传输据时候内存地址是否递增。
DMA_PeripheralDataSize 用来设置外设的数据长度是为字节传输（8bits），半字传(16bits) 还是字传输(32bits)。
DMA_MemoryDataSize 是用来设置内存的数据长度 (同上)

3. 使能相关外设的DMA传输
```c
USART_DMACmd(USART1,USART_DMAReq_Tx,ENABLE);//使能串口1DMA传输
```
4. 使能DMAx的相关通道。
```c
 //开启一次DMA传输
void MYDMA_Enable(DMA_Channel_TypeDef*DMA_CHx)
{ 
	DMA_Cmd(DMA_CHx, DISABLE );  //关闭USART1 TX DMA1 所指示的通道      
 	DMA_SetCurrDataCounter(DMA1_Channel4,DMA1_MEM_LEN);//重新设置DMA通道的DMA缓存的大小
 	DMA_Cmd(DMA_CHx, ENABLE);  //使能USART1 TX DMA1 所指示的通道 
}	  
```
5. 状态相关的函数。
```c
if(DMA_GetFlagStatus(DMA1_FLAG_TC4)!=RESET)	//判断DMA1通道4是否传输完成
	{
		DMA_ClearFlag(DMA1_FLAG_TC4);//清楚传输完成标记
		break; 
	}
```

```c
DMA_GetCurrDataCounter(DMA1_Channel4)//获取缓冲区中还有多少没有传输
```