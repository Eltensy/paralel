#include <iostream>
#include <cstdlib>
#include <ctime>
#include <cuda_runtime.h>
#include "device_launch_parameters.h"
#include <omp.h>


__global__ void matrixMultiplyKernel(int* A, int* B, int* result, int n, int m, int k) {
    int row = blockIdx.y * blockDim.y + threadIdx.y;
    int col = blockIdx.x * blockDim.x + threadIdx.x;

    if (row < n && col < n) {
        int value = 0;
        for (int i = 0; i < m; i++) {
            value += A[row * m + i] * B[i * k + col];
        }
        result[row * n + col] = value;
    }
}


void initializeMatrix(int* matrix, int rows, int cols) {
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            matrix[i * cols + j] = rand() % 10 + 1;
        }
    }
}

int main() {
    srand(static_cast<unsigned>(time(0)));

    cudaSetDevice(0);
    for(int n = 32; n<=16384; n = n*2)
    {
                
                std::cout << "n: " << n << std::endl;

                int m = n;
                int* A = new int[n * m];
                int* B = new int[m * n];
                int* resultCUDA = new int[n * n];


                initializeMatrix(A, n, m);
                initializeMatrix(B, m, n);

                int* d_A, * d_B, * d_result;
                cudaMalloc(&d_A, n * m * sizeof(int));
                cudaMalloc(&d_B, m * n * sizeof(int));
                cudaMalloc(&d_result, n * n * sizeof(int));

                cudaMemcpy(d_A, A, n * m * sizeof(int), cudaMemcpyHostToDevice);
                cudaMemcpy(d_B, B, m * n * sizeof(int), cudaMemcpyHostToDevice);

                dim3 blockSize(16, 16);
                dim3 gridSize((n + blockSize.x - 1) / blockSize.x, (n + blockSize.y - 1) / blockSize.y);

                clock_t cudaStart = clock();
                matrixMultiplyKernel << <gridSize, blockSize >> > (d_A, d_B, d_result, n, m, n);
                cudaDeviceSynchronize();
                clock_t cudaEnd = clock();

                std::cout << "CUDA algo run time: " << 1000.0 * (cudaEnd - cudaStart) / CLOCKS_PER_SEC << " ms" << std::endl;

                cudaMemcpy(resultCUDA, d_result, n * n * sizeof(int), cudaMemcpyDeviceToHost);

                //double speedup = (sequentialEnd - sequentialStart) / (cudaEnd - cudaStart);
                //std::cout << "Speedup: " << speedup << std::endl;

                delete[] A;
                delete[] B;
                delete[] resultCUDA;

                cudaFree(d_A);
                cudaFree(d_B);
                cudaFree(d_result);

    }
        return 0;
}
