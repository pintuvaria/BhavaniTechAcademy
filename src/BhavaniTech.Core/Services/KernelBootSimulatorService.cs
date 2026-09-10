using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public enum CpuExecutionMode
    {
        RealMode16,
        ProtectedMode32,
        LongMode64
    }

    public record GdtDescriptor(
        int Selector,
        string Name,
        uint BaseAddress,
        uint Limit,
        byte AccessByte,
        int RingLevel,
        string Description
    );

    public record IdtEntry(
        int VectorNumber,
        string Name,
        string Type, // Exception, Hardware IRQ, Software Syscall
        int RingLevel,
        string HandlerSymbol,
        string Description
    );

    public record InterruptDispatchResult(
        int Vector,
        string InterruptName,
        string Type,
        int PreviousRing,
        int CurrentRing,
        ulong ErrorCode,
        List<string> StackFrameIret,
        string ExecutionLog
    );

    public class KernelBootSimulatorService
    {
        public CpuExecutionMode CurrentMode { get; private set; } = CpuExecutionMode.RealMode16;
        public bool A20LineEnabled { get; private set; } = false;
        public uint Cr0 { get; private set; } = 0x00000010; // Real mode default
        public ulong Cr3 { get; private set; } = 0; // Page Directory Base
        public ulong Cr4 { get; private set; } = 0;
        public ulong EferMsr { get; private set; } = 0;
        public int CurrentRing { get; private set; } = 0; // 0 = Kernel, 3 = User

        public List<GdtDescriptor> GdtTable { get; } = new();
        public Dictionary<int, IdtEntry> IdtTable { get; } = new();

        public KernelBootSimulatorService()
        {
            InitializeGdt();
            InitializeIdt();
        }

        private void InitializeGdt()
        {
            GdtTable.Clear();
            GdtTable.Add(new GdtDescriptor(0x00, "Null Descriptor", 0x0, 0x0, 0x00, 0, "Mandatory CPU null descriptor for safety"));
            GdtTable.Add(new GdtDescriptor(0x08, "Kernel Code (Ring 0)", 0x0, 0xFFFFF, 0x9A, 0, "64-bit/32-bit executable supervisor segment"));
            GdtTable.Add(new GdtDescriptor(0x10, "Kernel Data (Ring 0)", 0x0, 0xFFFFF, 0x92, 0, "Read/write supervisor data segment"));
            GdtTable.Add(new GdtDescriptor(0x18, "User Code (Ring 3)", 0x0, 0xFFFFF, 0xFA, 3, "Userland executable segment with DPL=3"));
            GdtTable.Add(new GdtDescriptor(0x20, "User Data (Ring 3)", 0x0, 0xFFFFF, 0xF2, 3, "Userland read/write data segment with DPL=3"));
            GdtTable.Add(new GdtDescriptor(0x28, "Task State Segment (TSS)", 0x7E00, 0x67, 0x89, 0, "Stores Ring 0 RSP stack pointers for privilege transitions"));
        }

        private void InitializeIdt()
        {
            IdtTable.Clear();
            // CPU Exceptions (0-31)
            IdtTable[0x00] = new IdtEntry(0x00, "Divide by Zero (#DE)", "Fault", 0, "isr_divide_zero", "Triggered by DIV or IDIV instruction with zero divisor");
            IdtTable[0x06] = new IdtEntry(0x06, "Invalid Opcode (#UD)", "Fault", 0, "isr_invalid_opcode", "Processor attempted to execute undefined instruction byte");
            IdtTable[0x0D] = new IdtEntry(0x0D, "General Protection (#GP)", "Fault", 0, "isr_general_protection", "Privilege violation (e.g. Ring 3 executing CLI or invalid segment access)");
            IdtTable[0x0E] = new IdtEntry(0x0E, "Page Fault (#PF)", "Fault", 0, "isr_page_fault", "Access to non-present page or violation of page permissions (CR2 holds fault address)");

            // Hardware IRQs (Master/Slave 8259 PIC remapped to 0x20-0x2F)
            IdtTable[0x20] = new IdtEntry(0x20, "IRQ 0: System Timer (PIT)", "Hardware IRQ", 0, "irq0_timer_tick", "Periodic clock tick driving preemptive multitasking");
            IdtTable[0x21] = new IdtEntry(0x21, "IRQ 1: PS/2 Keyboard", "Hardware IRQ", 0, "irq1_keyboard", "Fires on key press/release scan-code arrival on Port 0x60");

            // Software Syscalls
            IdtTable[0x80] = new IdtEntry(0x80, "Software Syscall (int 0x80)", "Software Syscall", 3, "syscall_dispatcher", "Userland entry point into kernel services via EAX system call number");
        }

        public string TransitionToRealMode()
        {
            CurrentMode = CpuExecutionMode.RealMode16;
            A20LineEnabled = false;
            Cr0 = 0x00000010;
            Cr3 = 0;
            Cr4 = 0;
            EferMsr = 0;
            CurrentRing = 0;
            return "=== RESET: CPU Operating in 16-bit Real Mode (1 MB segmented addressing: CS:IP). Paging disabled. ===";
        }

        public string TransitionToProtectedMode()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== TRANSITION: 16-bit Real Mode -> 32-bit Protected Mode ===");
            sb.AppendLine("1. Disabling hardware interrupts: CLI");
            sb.AppendLine("2. Enabling A20 Gate via Keyboard Controller (Port 0x92 / Fast A20)");
            A20LineEnabled = true;

            sb.AppendLine("3. Loading Global Descriptor Table register: LGDT [gdt_descriptor]");
            sb.AppendLine($"   -> GDT contains {GdtTable.Count} descriptors (Kernel Code/Data, User Code/Data, TSS).");

            sb.AppendLine("4. Setting PE (Protection Enable) Bit 0 in CR0 register:");
            Cr0 |= 0x00000001; // Set PE bit
            sb.AppendLine($"   -> CR0 updated to 0x{Cr0:X8}");

            sb.AppendLine("5. Executing Far Jump to flush 16-bit instruction prefetch pipeline:");
            sb.AppendLine("   -> JMP 0x08:pmode_entry (0x08 = Kernel Code Selector)");

            CurrentMode = CpuExecutionMode.ProtectedMode32;
            sb.AppendLine("✅ CPU Successfully Operating in 32-bit Protected Mode (Flat 4 GB address space).");
            return sb.ToString();
        }

        public string TransitionToLongMode()
        {
            if (CurrentMode == CpuExecutionMode.RealMode16)
            {
                TransitionToProtectedMode();
            }

            var sb = new StringBuilder();
            sb.AppendLine("=== TRANSITION: 32-bit Protected Mode -> 64-bit Long Mode ===");
            sb.AppendLine("1. Disabling Paging bit in CR0: CR0.PG = 0");
            Cr0 &= ~0x80000000;

            sb.AppendLine("2. Enabling PAE (Physical Address Extension) in CR4 register:");
            Cr4 |= 0x00000020; // Set PAE bit
            sb.AppendLine($"   -> CR4 updated to 0x{Cr4:X8}");

            sb.AppendLine("3. Initializing 4-Level Paging Architecture in memory:");
            Cr3 = 0x00100000; // PML4 table base address (1 MB mark)
            sb.AppendLine($"   -> CR3 loaded with PML4 table base: 0x{Cr3:X16}");
            sb.AppendLine("   -> Level 4: PML4 (Page Map Level 4) -> 512 entries");
            sb.AppendLine("   -> Level 3: PDPT (Page Directory Pointer Table)");
            sb.AppendLine("   -> Level 2: PD (Page Directory) 2 MB Huge Pages");
            sb.AppendLine("   -> Level 1: PT (Page Table) 4 KB Standard Pages");

            sb.AppendLine("4. Enabling Long Mode in EFER (Extended Feature Enable MSR 0xC0000080):");
            EferMsr |= (1 << 8); // LME (Long Mode Enable)
            sb.AppendLine($"   -> EFER MSR updated to 0x{EferMsr:X16} (LME=1)");

            sb.AppendLine("5. Activating Paging in CR0: CR0.PG = 1");
            Cr0 |= 0x80000001; // PE + PG

            sb.AppendLine("6. Executing 64-bit Far Jump to 64-bit Code Segment: JMP 0x08:long_mode_entry");
            CurrentMode = CpuExecutionMode.LongMode64;
            sb.AppendLine("✅ CPU Successfully Switched to 64-bit Sub-mode (Long Mode active with 4-level paging).");
            return sb.ToString();
        }

        public InterruptDispatchResult TriggerInterrupt(int vector, ulong errorParam = 0)
        {
            var entry = IdtTable.GetValueOrDefault(vector) ?? new IdtEntry(vector, $"Interrupt 0x{vector:X2}", "Custom", 0, "isr_generic", "User-defined interrupt handler");

            int prevRing = CurrentRing;
            CurrentRing = 0; // CPU switches to Ring 0 upon entering ISR

            var stackFrame = new List<string>
            {
                $"[SS]  0x00000010 (Kernel Stack Segment)",
                $"[RSP] 0x7FFF0000 (Saved User Stack Pointer)",
                $"[RFLAGS] 0x00000202 (IF=1, Reserved=1)",
                $"[CS]  0x00000008 (Kernel Code Segment Selector)",
                $"[RIP] 0x004012A8 (Instruction Return Pointer)"
            };

            if (entry.Type == "Fault")
            {
                stackFrame.Insert(0, $"[ERR] 0x{errorParam:X16} (CPU Hardware Error Code)");
            }

            var sb = new StringBuilder();
            sb.AppendLine($"⚡ [IDT Interrupt Dispatch]: Vector 0x{vector:X2} -> {entry.Name} ({entry.Type})");
            sb.AppendLine($"Privilege Elevation : Ring {prevRing} -> Ring 0 (Kernel Supervisor)");
            sb.AppendLine($"Handler Entry Point : &{entry.HandlerSymbol}()");
            sb.AppendLine($"Description         : {entry.Description}");
            if (vector == 0x0E)
            {
                sb.AppendLine($"Page Fault Linear Address (CR2): 0x{errorParam:X16}");
            }
            else if (vector == 0x80)
            {
                sb.AppendLine("Syscall Dispatched: EAX=1 (sys_write), EBX=1 (stdout), ECX=buf_ptr, EDX=len");
            }

            return new InterruptDispatchResult(
                vector,
                entry.Name,
                entry.Type,
                prevRing,
                0,
                errorParam,
                stackFrame,
                sb.ToString()
            );
        }
    }
}
