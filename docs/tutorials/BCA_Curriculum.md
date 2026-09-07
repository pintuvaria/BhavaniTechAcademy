# BCA (Bachelor of Computer Applications) - Foundation Tutorial

## Module 1: C Programming & Memory Management
C is the foundational language of computer science.
- **Pointers:** A pointer is a variable that stores the exact memory address of another variable. Essential for low-level memory manipulation.
- **Dynamic Memory Allocation:** 
  - `malloc()`: Allocates a specific block of memory on the heap at runtime.
  - `free()`: Crucial for preventing memory leaks; releases the allocated block back to the system.
- **Structs:** Custom, composite data types that group related variables (e.g., creating a `Student` struct with a Name, ID, and GPA).

## Module 2: Data Structures
How data is stored and organized in memory dictates algorithm performance.
- **Arrays:** Contiguous blocks of memory. Fast read access `O(1)`, but slow to resize or insert elements.
- **Linked Lists:** Sequential nodes containing data and a pointer to the next node. Fast insertions, but slow read access `O(n)`.
- **Stacks (LIFO):** Last-In-First-Out. Like a stack of plates. Used heavily in CPU function call stacks and undo mechanisms.
- **Queues (FIFO):** First-In-First-Out. Like a line at a store. Used in task scheduling and network buffers.

## Module 3: Database Management Systems (DBMS)
- **The Relational Model:** Data organized into tables (relations) with Primary Keys and Foreign Keys.
- **Normalization:** The process of structuring a database to reduce redundancy and improve data integrity.
  - **1NF:** Eliminate repeating groups; ensure all attributes are atomic.
  - **2NF:** Remove partial dependencies.
  - **3NF:** Remove transitive dependencies (non-key columns depending on other non-key columns).
- **SQL (Structured Query Language):**
  - **DDL:** Data Definition Language (`CREATE`, `ALTER`, `DROP`).
  - **DML:** Data Manipulation Language (`SELECT`, `INSERT`, `UPDATE`, `DELETE`).

## Module 4: Web Technologies
- **HTML (HyperText Markup Language):** The structural skeleton of a webpage.
- **CSS (Cascading Style Sheets):** The visual design, layout (Flexbox/Grid), and responsiveness.
- **JavaScript:** The client-side logic layer. Interacts with the Document Object Model (DOM) to dynamically update the UI without reloading the page.
