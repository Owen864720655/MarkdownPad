
ARM：Advanced RISC Machine
AAPCS：ARM Architecture Process call standard
ARM 体系结构过程调用标准
RISC： Reduced Instruction Set Computer 精简指令集计算机
RTOS：Real Time Operating System 实时操作系统
DMA：Direct Memory Access 存储器直接访问
EXTI: External Interrupts 外部中断
FSMC: Flexible static memory controller 可变静态存储控制器
FPB：flash patch and breakpoint FLASH 转换及断电单元
HSE：Hign speed external
HSI: High speed internal
LSE: Low Speed external
LSI: Low Speed Internal
LSU: load store unit 存取单元
PFU: prefetch unit 预取单元
ISR：Interrupt Service Routines 中断服务程序
NMI: Nonmaskable Interrupt 不可屏蔽中断
NVIC: Nested Vectored Interrupt Controller 嵌套向量中断控制器
MPU: Memory Protection Unit 内存保护单元
MIPS:million instructions per second 每秒能执行的百万条指令的条数
RCC：Reset and clock control 复位和时钟控制
RTC: Real-Time Clock 实时时钟
IWDG: independent watchdog 独立看门狗
WWDG：Window watchdog 窗口看门狗
TIM：timer 定时器
GAL: generic array logic 通用阵列逻辑
PAL: programmable array logic 可编程阵列逻辑
ASIC: Application Specific Integrated Circuit 专用集成电路
FPGA: Field－Programmable Gate Array 现场可编程门阵列
CPLD: Complex Programmable Logic Device 可编程逻辑器件
端口
AFIO：alternate function IO 复用 IO 端口
GPIO：general purpose input/output 通用 IO 端口
IOP:（A-G）:IO port A - IO port G (例如：IOPA:IO port A)
CAN：Controller area network 控制器局域网
FLITF：The Flash memory interface 闪存存储器接口
I2C： Inter-integrated circuit 微集成电路
IIS： integrate interface of sound 集成音频接口
JTAG：joint test action group 联合测试行动小组
SPI：Serial Peripheral Interface 串行外围设备接口 SDIO: SD I/O
UART: Universal Synchr./Asynch. Receiver Transmitter 通用异步接收/发送装置
USB: Universal Serial Bus 通用串行总线
寄存器相关
CPSP： Current Program Status Register 当前程序状态寄存器
SPSP： saved program status register 程序状态备份寄存器
CSR：clock control/status register 时钟控制状态寄存器
LR： link register 链接寄存器
SP： stack pointer 堆栈指针
MSP: main stack pointer 主堆栈指针
PSP：process stack pointer 进程堆栈指针
PC： program counter 程序计数器
调试相关
ICE：in circuit emulator 在线仿真
ICE Breaker 嵌入式在线仿真单元
DBG：debug 调试
IDE：integrated development environment 集成开发环境
DWT: data watchpoint and trace 数据观测与跟踪单元
ITM: instrumentation trace macrocell 测量跟踪单元
ETM： embedded trace macrocell 嵌入式追踪宏单元
TPIU：trace port interface unit 跟踪端口接口单元
TAP： test access port 测试访问端口
DAP: debug access prot 调试访问端口
TP: trace port 跟踪端口
DP：debug port 调试端口
SWJ-DP: serial wire JTAG debug port 串行-JTAG 调试接口
SW-DP: serial wire debug port
串行调试接口
JTAG-DP：JTAG debug port
JTAG 调试接口
系统类
IRQ： interrupt request 中断请求
FIQ： fast interrupt request 快速中断请求
SW：software 软件
SWI： software interrupt 软中断
RO:read only 只读（部分）
RW:read write 读写（部分）
ZI:zero initial 零初始化（部分）
BSS：Block Started by Symbol 以符号开始的块（未初始化数据段）
总线
Bus Matrix 总线矩阵
Bus Splitter 总线分割
AHB-AP：advanced High-preformance Bus-access port
APB:advanced peripheral busAPB1: low speed APB
APB2: high speed APB
PPB： Private Peripheral Bus 专用外设总线
杂类
ALU：Arithmetic Logical Unit 算术逻辑单元
CLZ： count leading zero 前导零计数（指令）
SIMD： single instruction stream multiple data stream 单指令流，多数据流
VFP： vector floating point 矢量浮点运算
词汇/词组
Big Endian
大端存储模式
Little Endian 小端存储模式
context switch 任务切换（上下文切换）（CPU 寄存器内容的切换）
task switch 任务切换
literal pool 数据缓冲池
词汇类/单词
arbitration 仲裁
access 访问
assembler 汇编器
disassembly 反汇编
binutils 连接器
bit-banding 位段（技术）
bit-band alias 位段别名
bit-band region 位段区域
banked 分组
buffer 缓存/
ceramic 陶瓷
fetch 取指
decode 译码
execute 执行
Harvard 哈佛（架构）
handler 处理者
heap 堆
stack 栈
latency 延时
load (LDR) 加载（存储器内容加载到寄存器 Rn）
store (STR) 存储（寄存器 Rn 内容存储到存储器）
Loader 装载器
optimization 优化
process 进程/过程
thread 线程
prescaler 预分频器
prefetch 预读/预取指
perform 执行
pre-emption 抢占
tail-chaining 尾链
late-arriving 迟到
resonator 共振器
指令相关
instructions 指令
pseudo-instruction 伪指令
directive 伪操作
comments 注释
FA full ascending 满栈递增（方式）
EA empty ascending 空栈递增（方式）
FD full desending 满栈递减（方式）
ED empty desending 空栈递减（方式）
翻译
1.number of wait states for a read operation programmed on-the-fly
动态设置（programmed on-the-fly）的读操作的等待状态数目
参考文章
1.BSS 的参考：baike.baidu.com/view/453125.htm?fr=ala0_1
BSS 是 Unix 链接器产生的未初始化数据段。 其他的段分别是包含程序代码的“text”段和包含 已初始化数据的“data”段。BSS 段的变量只有名称和大小却没有值。此名后来被许多文件格 式使用，包括 PE。“以符号开始的块”指的是编译器处理未初始化数据的地方。BSS 节不包 含任何数据，只是简单的维护开始和结束的地址，以便内存区能在运行时被有效地清零。 BSS 节在应用程序的二进制映象文件中并不存在。
在采用段式内存管理的架构中 （比如 intel 的 80x86 系统）bss 段 ， （Block Started by Symbol segment）通常是指用来存放程序中未初始化的全局变量的一块内存区域，一般在初始化时 bss 段部分将会清零。bss 段属于静态内存分配，即程序一开始就将其清零了。
比如，在 C 语言之类的程序编译完成之后，已初始化的全局变量保存在.data 段中，未 初始化的全局变量保存在.bss 段中。
text 和 data 段都在可执行文件中（在嵌入式系统里一般是固化在镜像文件中），由系统 从可执行文件中加载；而 bss 段不在可执行文件中，由系统初始化。
2.ISR 的参考：baike.baidu.com/view/32247?fromTaglist
3.DMA 的参考：baike.baidu.com/view/32471.htm?fr=ala0_1
在实现 DMA 传输时，是由 DMA 控制器直接掌管总线，因此，存在着一个总线控制权转移 问题。即 DMA 传输前，CPU 要把总线控制权交给 DMA 控制器，而在结束 DMA 传输后， DMA 控制器应立即把总线控制权再交回给 CPU。
一个完整的 DMA 传输过程必须经过下面的 4 个步骤。
1.DMA 请求 CPU 对 DMA 控制器初始化，并向 I/O 接口发出操作命令，I/O 接口提出 DMA 请求。
2.DMA 响应 DMA 控制器对 DMA 请求判别优选级及屏蔽，向总线裁决逻辑提出总线 请求。当 CPU 执行完当前总线周期即可释放总线控制权。此时，总线裁决逻辑输出总线应 答，表示 DMA 已经响应，通过 DMA 控制器通知 I/O 接口开始 DMA 传输。
3.DMA 传输 DMA 控制器获得总线控制权后，CPU 即刻挂起或只执行内部操作，由 DMA 控制器输出读写命令，直接控制 RAM 与 I/O 接口进行 DMA 传输。
4.DMA 结束当完成规定的成批数据传送后，DMA 控制器即释放总线控制权，并向 I/O 接口发出结束信号。当 I/O 接口收到结束信号后，一方面停止 I/O 设备的工作，另一方面向 CPU 提出中断请求，使 CPU 从不介入的状态解脱，并执行一段检查本次 DMA 传输操作正 确性的代码。最后，带着本次操作结果及状态继续执行原来的程序。
由此可见， DMA 传输方式无需 CPU 直接控制传输， 也没有中断处理方式那样保留现场 和恢复现场的过程，通过硬件为 RAM 与 I/O 设备开辟一条直接传送数据的通路，使 CPU 的效率大为提高。
