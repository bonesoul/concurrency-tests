# Concurrency Tests [![Build status](https://ci.appveyor.com/api/projects/status?id=bbli6p8ochrxl5ji)](https://ci.appveyor.com/project/concurrency-tests)

Performance tests for different concurrency models. Coded as an answer for [Should i use ThreadPools or Task Parallel Library for IO-bound operations?](http://stackoverflow.com/questions/5213695/should-i-use-threadpools-or-task-parallel-library-for-io-bound-operations)

**Test Legend**
* Itr: Iteration
* Seq: Sequential Approach.
* PrlEx: Parallel Extensions - Parallel.ForEach
* TPL: Task Parallel Library
* TPool: ThreadPool

**Sample Test Results**

```
Single-Core CPU [Win7-32] -- runs under VMWare --

Test Environment: 1 physical cpus, 1 cores, 1 logical cpus.
Will be parsing a total of 10 feeds.
________________________________________________________________________________

Itr.    Seq.    PrlEx   TPL     TPool
________________________________________________________________________________

#1      10.82s  04.05s  02.69s  02.60s
#2      07.48s  03.18s  03.17s  02.91s
#3      07.66s  03.21s  01.90s  01.68s
#4      07.43s  01.65s  01.70s  01.76s
#5      07.81s  02.20s  01.75s  01.71s
#6      07.67s  03.25s  01.97s  01.63s
#7      08.14s  01.77s  01.72s  02.66s
#8      08.04s  03.01s  02.03s  01.75s
#9      08.80s  01.71s  01.67s  01.75s
#10     10.19s  02.23s  01.62s  01.74s
________________________________________________________________________________

Avg.    08.40s  02.63s  02.02s  02.02s
________________________________________________________________________________
```

```
Single-Core CPU [WinXP] -- runs under VMWare --

Test Environment: 1 physical cpus, NotSupported cores, NotSupported logical cpus.
Will be parsing a total of 10 feeds.
________________________________________________________________________________

Itr.    Seq.    PrlEx   TPL     TPool
________________________________________________________________________________

#1      10.79s  04.05s  02.75s  02.13s
#2      07.53s  02.84s  02.08s  02.07s
#3      07.79s  03.74s  02.04s  02.07s
#4      08.28s  02.88s  02.73s  03.43s
#5      07.55s  02.59s  03.99s  03.19s
#6      07.50s  02.90s  02.83s  02.29s
#7      07.80s  04.32s  02.78s  02.67s
#8      07.65s  03.10s  02.07s  02.53s
#9      10.70s  02.61s  02.04s  02.10s
#10     08.98s  02.88s  02.09s  02.16s
________________________________________________________________________________

Avg.    08.46s  03.19s  02.54s  02.46s
________________________________________________________________________________
```
```
Dual-Core CPU [Win7-64]

Test Environment: 1 physical cpus, 2 cores, 2 logical cpus.
Will be parsing a total of 10 feeds.
________________________________________________________________________________

Itr.    Seq.    PrlEx   TPL     TPool
________________________________________________________________________________

#1      07.09s  02.28s  02.64s  01.79s
#2      06.04s  02.53s  01.96s  01.94s
#3      05.84s  02.18s  02.08s  02.34s
#4      06.00s  01.43s  01.69s  01.43s
#5      05.74s  01.61s  01.36s  01.49s
#6      05.92s  01.59s  01.73s  01.50s
#7      06.09s  01.44s  02.14s  02.37s
#8      06.37s  01.34s  01.46s  01.36s
#9      06.57s  01.30s  01.58s  01.67s
#10     06.06s  01.95s  02.88s  01.62s
________________________________________________________________________________

Avg.    06.17s  01.76s  01.95s  01.75s
________________________________________________________________________________
```
```
Quad-Core CPU [Win7-64] -- HyprerThreading Supported --

Test Environment: 1 physical cpus, 4 cores, 8 logical cpus.
Will be parsing a total of 10 feeds.
________________________________________________________________________________

Itr.    Seq.    PrlEx   TPL     TPool
________________________________________________________________________________

#1      10.56s  02.03s  01.71s  01.69s
#2      07.42s  01.63s  01.71s  01.69s
#3      11.66s  01.69s  01.73s  01.61s
#4      07.52s  01.77s  01.63s  01.65s
#5      07.69s  02.32s  01.67s  01.62s
#6      07.31s  01.64s  01.53s  02.17s
#7      07.44s  02.56s  02.35s  02.31s
#8      08.36s  01.93s  01.73s  01.66s
#9      07.92s  02.15s  01.72s  01.65s
#10     07.60s  02.14s  01.68s  01.68s
________________________________________________________________________________

Avg.    08.35s  01.99s  01.75s  01.77s
________________________________________________________________________________
```
**Summarization**

Whether you run on a single-core environment or a multi-core one, Parallel Extensions, TPL and ThreadPool behaves the same and gives approximate results.
Still TPL has advantages like easy exception handling, cancellation support and ability to easily return Task results. Though Parallel Extensions is also another viable alternative.

Check [stackoverflow](http://stackoverflow.com/questions/5213695/should-i-use-threadpools-or-task-parallel-library-for-io-bound-operations) for further details.
