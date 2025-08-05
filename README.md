# Linear Algebra Library for Unity

## Description
A linear algebra library for Unity, fully written in Burst. It's designed to be a natural extension of Unity.Mathematics, offering a bit more of functionalities. Currently in an experimental stage and not yet ready for production use.

## Installation

To open the repo in Unity, follow these steps:

1. Clone the repo
2. Open the project in Unity

To use library in your own project:

1. Clone the repo to separate project
2. Copy Assets/LinearAlgebra/Source into your own project

## Usage

Here's a simple example:

This fork has a lot of changes to the floatN, floatMxN, doubleN, doubleMxN code from the upstream repository. The integer and bool vectors and matrices have hardly been touched.

I would recommend adding the  LINALG_DEBUG  define to your debug build. It adds a lot of checks.

Be very careful with memory management when using this library. It stores vectors and matrices in two buffers: persistent and temporary. Be aware which of your variables are persistent and temporary.

It is fast but I would recommend sticking to some careful patterns/rules. 

1. Do not call vector or matrix constructors directly. Alwayse Create them through the Arena or via operations.
2. Do not dispose of vectors or matrices yourself. Call the arena directly.
3. Be aware which of your variables are temp and persistent (Assert this). If you have a temporary result that you want to be persistent then copy the result to a persistent variable.
3. Check that arena.AllocationCount is not growing unexpectedly. This is persistent memory. You
should be able to account for all allocations.
4. ClearTemp frequently. This deallocates memory. Don't access any old temp allocated variables after ClearTemp.
5. Don't use the assignement operator with persistent variables other than for Arena creation. Use in-place (_inpl) functions to assign results to these variables. 


```csharp
    // memory management struct
    var arena = new Arena(Allocator.Persistent);

    int dim = 128;
    // creates a zero vector of 128 dimensions 
    floatN vecA = arena.floatVec(dim);
    // creates a vector of 128 dimensions with all elements set to 1
    floatN vecB = arena.floatVec(dim, 1f);

    // add per component (will allocate a new temporary vec)
    floatN vecAdd = vecA + vecB;
    // mul per component (will allocate a new temporary vec)
    floatN vecMul = vecA * vecB;

    // create identity matrix
    floatMxN matI = arena.floatIdentityMatrix(16);
    floatMxN matRand = arena.floatRandomMatrix(16, 16);

    // per component sum, allocates new temporary matrix
    floatMxN compSumMat = matI + matRand;

    // adds 1f to compSumMat inplace, allocating nothing
    floatOP.addInpl(compSumMat, 1f);

    // mulls matI into compSumMat inplace, allocating nothing 
    floatOP.compMulInpl(compSumMat, matI);

    // creates random matrix with range from -3f to 3f
    floatMxN A = arena.floatRandomDiagonalMatrix(dim, -3f, 3f);
    floatMxN B = arena.floatRandomDiagonalMatrix(dim, -3f, 3f);

    // dot multiply A and B, will allocate new temporary matrix
    floatMxN Ctmp = floatOP.dot(A, B);
    
    // make a persistent copy of this result
    floatMxN C = arena.CopyPersistent(Ctmp);
    
    {
        // DON'T DO THIS
        // Don't use assignement operator  =  with persistent mats and vecs. It can 
        // quietly convert them from persistent allocated to temp allocated or leak memory.
        C = A + B;  // C used to point to persistent allocated matrix. Now it points to temporary.
        arena.ClearTemp() // C's new memory just got freed
        C[0,1] = 5;    // now we are writing to unallocated memory. Very very bad.
    }
    
    {
        // Best Practice. Allocate all persistent variables ahead of time
        floatMxN AA = arena.floatMat(1,2);
        floatMxN BB = arena.floatMat(1,2);
        floatMxN CC = arena.floatMat(1,2);
        
        // Record number of allocations
        int expectedAllocations = arena.AllocationCount;
        ...
        {
            // Use temporary allocations for intermediate results
            // scope the tempoary variable in {} so the variables disappear out of scope
            // after calcs.
            floatMxN Dtmp = AA * CC + BB * CC;
            ... some calculations ...
            
            // Copy temporary result to persistent
            CC.Copy(Dtmp);
            
            // Clean up temporary variables
            arena.ClearTemp();
            Assert(arena.AllocationCount == expectedAllocations);
        }
    }
    
    // adds 5f to element on [0, 0] coords
    C[0, 0] += 5f;

    floatN b = arena.floatVec(dim, 1f);
    floatN x_result = arena.floatVec(dim, 1f);


    // solves linear system Ax = b inplace using QR, will allocate nothing permament
    // but will modify A and b
    OrthoOP.qrDirectSolve(ref A, ref b, ref x_result);

    // calculate L1 norm
    float norm = floatNormsOP.L1(x_result);

    // prints C matrix, although it will be cutoff because of big dimensions
    Print.Log(C);

    // returns true for all elements c_ij > a_ij, else false
    // will allocate
    boolMxN matCompare = C > A;

    // flips booleans, will allocate
    matCompare = !matCompare;

    // creates 3 new allocations
    boolMxN matCompare2 = C > A | C < B;

    // clears all temporary allocations
    arena.ClearTemp();

    // creates new int vector with dimensions of 10 and valued at 32
    intN intVec = arena.intVec(10, 32);

    // applies bitwise OR to elements, allocates new vector
    intVec |= 64;

    // also allocates, inplace methods do exist though
    intVec = 2 + (intVec << 2) + intVec;

    // creates new integer matrix
    intMxN intMat = arena.intRandomMatrix(10, 10, 0, 10);

    // creates new double matrix
    doubleMxN doubleMat = arena.doubleRandomMatrix(10, 10, 0, 10);

    // creates new short matrix
    shortMxN shortMat = arena.shortRandomMatrix(10, 10, 0, 10);

    // creates new long matrix
    longMxN longMat = arena.longRandomMatrix(10, 10, 0, 10);

    // mean of a vec
    double mean = doubleStatsOP.mean(in doubleMat);

    // mean of a vec
    double max = doubleStatsOP.max(in doubleMat);

    // vector of means of each row
    doubleN rowMean = doubleStatsOP.rowMean(in doubleMat);

    // clears and dispose all allocated vectors/matrices, disposes also arena
    arena.Dispose();
```

## Features

- ✅ Basic Unity.mathematics operations
- ✅ float, double, int, short, long, bool vectors and matrices
- ✅ Some basic statistics
- ✅ Basic vector-vector, vector-matrix, matrix-vector and matrix-matrix operations
- ✅ QR decomposition & solver for well-determined and over-determined systems
- ✅ Pivoting
- 🔳 LU decomposition & solver
- 🔳 View/Slice 
- 🔳 Find/Query operations (e.g.: find row with biggest L2 norm)
- 🔳 SVD decomposition
- 🔳 Optimizers? (min/max of function, gradient descent, root finding.. )

## TODO
- Better arena management and standalone vec/mat management (without arena allocation)
- Test arena/vec/mat allocated outside jobs (On normal C# thread)
- Refactor, unify the names / simplify
- More safety checks
- Vec/Mat views (simple structs for easier read/write)
- More stats functions and tests
- More solvers (LU, Pivoted LU)
- SVD
- Least squares
- Sparse matrix?
- Documentation

