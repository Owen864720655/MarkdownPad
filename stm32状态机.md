
下位机解析程序
```c
//保存数据
//USART_RX_STA


#define USART_REC_LEN  			200  	//定义接收的字符个数


#define START        0//开始接收
#define RECEVED_FF   1//接收到FF
#define RECEVED_FFFF   2//接收到FFFF
#define RECEVED_END_FF   3 //确认开始后接收到FF
#define SUCCESS      4//确认开始后接收到F7

extern u16 USART_RX_state;         		//接收状态标记
extern u16 USART_RX_STA;         		//已接收长度标记
extern u8 USART_RX_BUF[USART_REC_LEN];  //接收的缓冲区
//----------------------------------
u8 USART_RX_BUF[USART_REC_LEN];   
u16 USART_RX_STA=0;       	  
u16 USART_RX_state=0;       
void store_value(uint8_t value)
{
			USART_RX_BUF[USART_RX_STA&0X3FFF]=value ;
			USART_RX_STA++;

			USART_RX_STA = USART_RX_STA&0X3FFF;

			if(USART_RX_STA>(USART_REC_LEN-1)){
		    	USART_RX_STA = 0;//接收数据超过长度  
			}
}
//------------------接收中断
void USART1_IRQHandler(void)                	//串口1中断服务程序
	{
	u8 Res;
#ifdef OS_TICKS_PER_SEC	 	//如果时钟节拍数定义了,说明要使用ucosII了.
	OSIntEnter();    
#endif
	if(USART_GetITStatus(USART1, USART_IT_RXNE) != RESET)  //接收中断(接收到的数据必须是0x0d 0x0a结尾)当有数据接收的时候触发中断
		{
		Res =USART_ReceiveData(USART1);//(USART1->DR);	//读取接收到的数据

		switch(USART_RX_state)
		{

			case START:
			{

				if(Res==0xFF)
				{
					USART_RX_state = RECEVED_FF;
				}

				 break;
			}
			case RECEVED_FF:
			{
				
				if(Res==0xFF)
				 {
				 
				 	USART_RX_state = RECEVED_FFFF;
				 }
				 else USART_RX_state=START;
				 break;
			}
			//已经接收了FFFF
			case RECEVED_FFFF:
			{
				//先清空缓冲区
				//memset(USART_RX_BUF,0,USART_RX_STA);
				//USART_RX_STA=0;
				store_value(Res);
				if (Res==0xFF)
				{
					USART_RX_state=RECEVED_END_FF;
				}
				break;
			}
			case RECEVED_END_FF:
			{
				store_value(Res);
				if (Res==0xF7)
				{
					USART_RX_STA = USART_RX_STA - 2;
					USART_RX_state=SUCCESS;
				}
				else if (Res==0xFF)
				{
					USART_RX_state=RECEVED_END_FF;
				}
				else USART_RX_state=RECEVED_FFFF;

			}
			case SUCCESS:
			{
				
				break;
			}
			default:
			{
				break;
			}
}
}
}
//一些记录

```



运行字段相关参数
```c#
		textBox_SettingTime

		textBox_PSouce

		textBox_PumpPowerMax1

		textBox_PumpPowerMax2

		textBox_PumpPowerMax3

		textBox_PumpPowerMax4

		textBox_PumpPowerMax5

		textBox_PumpPowerMax6

		textBox_SettingPressure

		textBox_SettingBoostPressure

		textBox_InletPressureLowest

		textBox_PipePressureLowest

		textBox_OutletPressureMaximum

		textBox_BoostPressureMaximum

		textBox_MasterInverterFrequencyMaximum

		textBox_MasterInverterPowerLowest

		textBox_MasterECMaximum

		textBox_SubInverterFrequencyMaximum
  
		textBox_SubInverterPowerLowest

		textBox_SubECMaximum

		textBox_TimingRotationTime
		
		textBox_PressureFullscale

		textBox_LiquidlevelLowest

		textBox_AmbientTempMaximum

		textBox_AmbientHumidityMaximum

		textBox_ InletPressureThreshold

		textBox_PipePressureThreshold

		textBox_OutletPressureThreshold

		textBox_MasterFrequencyThreshold

		textBox_MasterPowerVThreshold

		textBox_MasterECThreshold

		textBox_SubFrequencyThreshold

		textBox_SubPowerVThreshold

		textBox_SubECThreshold

		textBox_FlowVThreshold
 
		textBox_LiquidlevelThreshold
	  
		textBox_AmbientTempThreshold

		textBox_AmbientHumidityThreshold
```

类型获取值记录
```c#

		m_CollectorPB.SettingTime = 
		m_CollectorPB.PSouce = 
		m_CollectorPB.PumpPowerMax1 = 
		m_CollectorPB.PumpPowerMax2 = 
		m_CollectorPB.PumpPowerMax3 = 
		m_CollectorPB.PumpPowerMax4 = 
		m_CollectorPB.PumpPowerMax5 = 
		m_CollectorPB.PumpPowerMax6 = 
		m_CollectorPB.SettingPressure = 
		m_CollectorPB.SettingBoostPressure = 
		m_CollectorPB.InletPressureLowest = 
		m_CollectorPB.PipePressureLowest = 
		m_CollectorPB.OutletPressureMaximum = 
		m_CollectorPB.BoostPressureMaximum = 
		m_CollectorPB.MasterInverterFrequencyMaximum = 
		m_CollectorPB.MasterInverterPowerLowest = 
		m_CollectorPB.MasterECMaximum = 
		m_CollectorPB.SubInverterFrequencyMaximum = 
		m_CollectorPB.SubInverterPowerLowest = 
		m_CollectorPB.SubECMaximum = 
		m_CollectorPB.TimingRotationTime = 
		m_CollectorPB.PressureFullscale = 
		m_CollectorPB.LiquidlevelLowest = 
		m_CollectorPB.AmbientTempMaximum = 
		m_CollectorPB.AmbientHumidityMaximum = 
		m_CollectorPB. InletPressureThreshold = 
		m_CollectorPB.PipePressureThreshold = 
		m_CollectorPB.OutletPressureThreshold = 
		m_CollectorPB.MasterFrequencyThreshold = 
		m_CollectorPB.MasterPowerVThreshold = 
		m_CollectorPB.MasterECThreshold = 
		m_CollectorPB.SubFrequencyThreshold = 
		m_CollectorPB.SubPowerVThreshold = 
		m_CollectorPB.SubECThreshold = 
		m_CollectorPB.FlowVThreshold = 
		m_CollectorPB.LiquidlevelThreshold = 
		m_CollectorPB.AmbientTempThreshold = 
	    m_CollectorPB.AmbientHumidityThreshold = 

		
```

结合起来
```c#
 //yth更改2016/05/25
                    
                 //设置时间
                textBox_SettingTime.Text = m_CollectorPB.SettingTime.ToString("yyMMddhhmmss");
                //参数来源
                textBox_PSouce.Text = m_CollectorPB.PSouce.ToString();
                //泵1额定功率
                textBox_PumpPowerMax1.Text = m_CollectorPB.PumpPowerMax1.ToString();
                //泵2额定功率
                textBox_PumpPowerMax2.Text = m_CollectorPB.PumpPowerMax2.ToString();
                //泵3额定功率
                textBox_PumpPowerMax3.Text = m_CollectorPB.PumpPowerMax3.ToString();
                //泵4额定功率
                textBox_PumpPowerMax4.Text = m_CollectorPB.PumpPowerMax4.ToString();
                //泵5额定功率(辅泵)
                textBox_PumpPowerMax5.Text = m_CollectorPB.PumpPowerMax5.ToString();
                //泵6额定功率
                textBox_PumpPowerMax6.Text = m_CollectorPB.PumpPowerMax6.ToString();
                //设定压力
                textBox_SettingPressure.Text = m_CollectorPB.SettingPressure.ToString();
                //设定增压压力(有增压泵时使用)
                textBox_SettingBoostPressure.Text = m_CollectorPB.SettingBoostPressure.ToString();
                ////进水压力低限
                textBox_InletPressureLowest.Text = m_CollectorPB.InletPressureLowest.ToString();
                //进水压力(自来水)低限(箱式时使用)
                textBox_PipePressureLowest.Text = m_CollectorPB.PipePressureLowest.ToString();
                //出水压力高限
                textBox_OutletPressureMaximum.Text = m_CollectorPB.OutletPressureMaximum.ToString();
                //出水压力(增压)高限（有增压泵时使用）
                textBox_BoostPressureMaximum.Text = m_CollectorPB.BoostPressureMaximum.ToString();
                //主变频器频率高限
                textBox_MasterInverterFrequencyMaximum.Text = m_CollectorPB.MasterInverterFrequencyMaximum.ToString();
                //主变频器功率低限
                textBox_MasterInverterPowerLowest.Text = m_CollectorPB.MasterInverterPowerLowest.ToString();
                //主电流高限
                textBox_MasterECMaximum.Text = m_CollectorPB.MasterECMaximum.ToString();
                //辅变频器频率高限
                textBox_SubInverterFrequencyMaximum.Text = m_CollectorPB.SubInverterFrequencyMaximum.ToString();
                //辅变频器功率低限
                textBox_SubInverterPowerLowest.Text = m_CollectorPB.SubInverterPowerLowest.ToString();
                //辅电流高限
                textBox_SubECMaximum.Text = m_CollectorPB.SubECMaximum.ToString();
                //定时轮换时间
                textBox_TimingRotationTime.Text = m_CollectorPB.TimingRotationTime.ToString();
                //出水压力表量程
                textBox_PressureFullscale.Text = m_CollectorPB.PressureFullscale.ToString();
                //液位低限
                textBox_LiquidlevelLowest.Text = m_CollectorPB.LiquidlevelLowest.ToString();
                //环境温度高限
                textBox_AmbientTempMaximum.Text = m_CollectorPB.AmbientTempMaximum.ToString();
                //环境湿度高限
                textBox_AmbientHumidityMaximum.Text = m_CollectorPB.AmbientHumidityMaximum.ToString();
                //进口压力阈值
                textBox_InletPressureThreshold.Text = m_CollectorPB.InletPressureThreshold.ToString();
                //进口(自来水)压力阈值,使用于箱式
                textBox_PipePressureThreshold.Text = m_CollectorPB.PipePressureThreshold.ToString();
                //出口压力阈值
                textBox_OutletPressureThreshold.Text = m_CollectorPB.OutletPressureThreshold.ToString();
                //主变频器频率阈值
                textBox_MasterFrequencyThreshold.Text = m_CollectorPB.MasterFrequencyThreshold.ToString();
                //主变频器功率阈值
                textBox_MasterPowerVThreshold.Text = m_CollectorPB.MasterPowerVThreshold.ToString();
                //主电流阈值
                textBox_MasterECThreshold.Text = m_CollectorPB.MasterECThreshold.ToString();
                //辅变频器频率阈值
                textBox_SubFrequencyThreshold.Text = m_CollectorPB.SubFrequencyThreshold.ToString();
                //辅变频器功率阈值
                textBox_SubPowerVThreshold.Text = m_CollectorPB.SubPowerVThreshold.ToString();
                //辅电流阈值
                textBox_SubECThreshold.Text = m_CollectorPB.SubECThreshold.ToString();
                //流量阈值
                textBox_FlowVThreshold.Text = m_CollectorPB.FlowVThreshold.ToString();
                //液位阈值
                textBox_LiquidlevelThreshold.Text = m_CollectorPB.LiquidlevelThreshold.ToString();
                //环境温度阈值
                textBox_AmbientTempThreshold.Text = m_CollectorPB.AmbientTempThreshold.ToString();
                //环境湿度阈值
                textBox_AmbientHumidityThreshold.Text = m_CollectorPB.AmbientHumidityThreshold.ToString();
                
```

^(m_\w*).(\w*) =
$1.$2 = double.Parse(textBox_$2.Text.Trim());\n



读取： FF FF 00 00 06 00 00 FF FF 00 00 01 12 42 43 BD 5D FF F7
时间同步: FF FF 00 00 0D 00 00 FF FF 00 00 01 02 20 16 05 26 16 07 58 D0 C7 95 49 FF F7
取时:FF FF 00 00 06 00 00 FF FF 00 00 01 01 EB BE EF 1C FF F7

