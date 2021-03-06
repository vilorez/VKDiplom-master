#include "stdafx.h"
#include "MulVsDiv.h"
#include <ostream>
#include <iostream>
#include <locale>
#include <vector>
#include <numeric>
#include <tchar.h>
#include <windows.h>
#include "StopWatch.h"

void MulVsDiv::ResetArrays(const int length, double* a, double* b, double& ignoreit)
{
	ignoreit += a[(rand() % (int)(length))] + b[(rand() % (int)(length))];
	for (size_t i = 0; i < length; i++)
	{
		a[i] = 6 * ((double)rand() / (RAND_MAX)) + 2;
		b[i] = 2 * ((double)rand() / (RAND_MAX)) + 1;
	}
	ignoreit += a[(rand() % (int)(length))] + b[(rand() % (int)(length))];
	ignoreit = 1/ignoreit;
}

void MulVsDiv::Loop()
{
	StopWatch sw;

	const int length = 256;
	const int loops = 1e7;
	std::cout << "Simple loop:\n---" << std::endl;
	double a[length], b[length], c[length];
	auto ignoreit = 0.0;

	ResetArrays(length, a, b, ignoreit);
	sw.Start();
	for (size_t l = 0; l < loops; l++)
	{
		// MSVC cannot vectorize this loop (message 1300).
		// However, if this loop will not be nested in, autovectorization will happen.
		// Same condition apply for mul/div/rcp loops

		// ICL does not have this issue, but to provide both vectorized and nonvectorized comparison 
		// i specifically disabled vectorization in this method
#pragma novector
#pragma loop( no_vector )
		for (int i = 0; i < length; ++i)
		{
			c[i] = a[i] + b[i];
		}
	}
	sw.Stop();
	auto add_time = sw.EllapsedTime();
	std::cout << "Addition: " << add_time << std::endl;
	ignoreit -= c[(rand() % static_cast<int>(length))];
	ResetArrays(length, a, b, ignoreit);

	sw.Start();
	for (size_t l = 0; l < loops; l++)
	{
#pragma novector
#pragma loop( no_vector )
		for (int i = 0; i < length; ++i)
		{
			c[i] = a[i] * b[i];
		}
	}
	sw.Stop();
	auto mul_time = sw.EllapsedTime();
	std::cout << "Multiplication: " << mul_time << std::endl;
	ignoreit -= c[(rand() % static_cast<int>(length))];
	ResetArrays(length, a, b, ignoreit);

	sw.Start();
	for (size_t l = 0; l < loops; l++)
	{
#pragma novector
#pragma loop( no_vector )
		for (int i = 0; i < length; ++i)
		{
			c[i] = a[i] / b[i];
		}
	}
	sw.Stop();
	auto div_time = sw.EllapsedTime();
	ignoreit -= c[(rand() % static_cast<int>(length))];
	ignoreit += a[(rand() % static_cast<int>(length))] + b[(rand() % static_cast<int>(length))];
	std::cout << "Division: " << div_time << std::endl;
	std::cout << "Addition faster than multiplication: " << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl;
	std::cout << "Just ignore it: " << ignoreit << std::endl << std::endl;
}

void MulVsDiv::LoopVectorized()
{
	const int length = 256;
	const int loops = 1e6;
	std::cout << "Vectorized loop:\n---" << std::endl;
	double a[length], b[length],c[length];
	auto ignoreit = 0.0;

	ResetArrays(length, a, b, ignoreit);
	int l = 0;
	auto start = clock();
add:
	for (int i = 0; i < length; i++)
	{
		c[i] = a[i] + b[i];
	}
	// MSVC doesn't vectorize nested loops (message 1300 - too little computation to vectorize) as mentioned on line 23 in function 'Loop'.
	// However if nested loop is replaced with this nasty workaround SIMD vectorization will happen.
	// Same condition apply for mul/div/rcp loops
	while (l < loops)
	{
		++l;
		goto add;
	}
	auto add_time = clock() - start;
	std::cout << "Addition: " << add_time << std::endl;
	ignoreit -= c[(rand() % (int)(length))];
	ResetArrays(length, a, b, ignoreit);

	l = 0;
	start = clock();
mul:
	for (int i = 0; i < length; ++i)
	{
		c[i] = a[i] * b[i];
	}
	while (l < loops)
	{
		++l;
		goto mul;
	}
	auto mul_time = clock() - start;
	std::cout << "Multiplication: " << mul_time << std::endl;
	ignoreit -= c[(rand() % (int)(length))];
	ResetArrays(length, a, b, ignoreit);

	l = 0;
	start = clock();
div:
	for (int i = 0; i < length; ++i)
	{
		c[i] = a[i] / b[i];
	}
	while (l < loops)
	{
		++l;
		goto div;
	}
	auto div_time = clock() - start;
	ignoreit -= c[(rand() % (int)(length))];
	ignoreit += a[(rand() % (int)(length))] + b[(rand() % (int)(length))];
	std::cout << "Division: " << div_time << std::endl;
	std::cout << "Addition faster than multiplication: " << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl;
	std::cout << "Just ignore it: " << ignoreit << std::endl << std::endl;
}

void MulVsDiv::DynamicArrayLoop()
{
	const int length = 256;
	const int loops = 1e6;
	std::cout << "Dynamic array loop:\n---" << std::endl;
	std::vector<double> av(length), bv(length), cv(length);
	double *a = &av.front(), *b = &bv.front(), *c = &cv.front();
	auto ignoreit = 0.0;

	ResetArrays(length, a, b, ignoreit);

	
	auto start = clock();
	for (size_t l = 0; l < loops; l++)
	{
		// MSVC cannot vectorize this loop (message 1300).
		// However, if this loop will not be nested in, autovectorization will happen.
		// Same condition apply for mul/div/rcp loops

		// ICL does not have this issue, but to provide both vectorized and nonvectorized comparison 
		// i specifically disabled vectorization in this method
#pragma novector
#pragma loop( no_vector )
		for (int i = 0; i < length; ++i)
		{
			c[i] = a[i] + b[i];
		}
	}
	auto add_time = clock() - start;
	std::cout << "Addition: " << add_time << std::endl;

	ResetArrays(length, a, b, ignoreit);
	ignoreit /= c[(rand() % (int)(length))];

	start = clock();
	for (size_t l = 0; l < loops; l++)
	{
#pragma novector
#pragma loop( no_vector )
		for (int i = 0; i < length; ++i)
		{
			c[i] = a[i] * b[i];
		}
	}
	auto mul_time = clock() - start;
	std::cout << "Multiplication: " << mul_time << std::endl;

	ResetArrays(length, a, b, ignoreit);
	ignoreit -= c[(rand() % (int)(length))];
	start = clock();
	for (size_t l = 0; l < loops; l++)
	{
#pragma novector
#pragma loop( no_vector )
		for (int i = 0; i < length; ++i)
		{
			c[i] = a[i] / b[i];
		}
	}
	auto div_time = clock() - start;
	ignoreit += a[(rand() % (int)(length))] + b[(rand() % (int)(length))];
	ignoreit /= c[(rand() % (int)(length))];
	std::cout << "Division: " << div_time << std::endl;
	std::cout << "Addition faster than multiplication: " << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl;
	std::cout << "Just ignore it: " << ignoreit << std::endl << std::endl;
}

void MulVsDiv::DynamicArrayLoopVectorized()
{
	const int length = 256;
	const int loops = 1e6;
	std::cout << "Vectorized dynamic array loop:\n---" << std::endl;
	std::vector<double> av(length), bv(length), cv(length);
	double *a = &av.front(), *b = &bv.front(), *c = &bv.front();
	auto ignoreit = 0.0;

	ResetArrays(length, a, b, ignoreit);

	int l = 0;
	auto start = clock();
add:
	for (int i = 0; i < length; i++)
	{
		c[i] = a[i] + b[i];
	}
	// MSVC doesn't vectorize nested loops (message 1300 - too little computation to vectorize) in function 'DynamicArrayLoop'.
	// However if nested loop is replaced with this nasty workaround SIMD vectorization will happen.
	// Same condition apply for mul/div/rcp loops
	while (l < loops)
	{
		++l;
		goto add;
	}
	auto add_time = clock() - start;
	std::cout << "Addition: " << add_time << std::endl;

	ResetArrays(length, a, b, ignoreit);
	ignoreit /= c[(rand() % (int)(length))];
	l = 0;
	start = clock();
mul:
	for (int i = 0; i < length; ++i)
	{
		c[i] = a[i] * b[i];
	}
	while (l < loops)
	{
		++l;
		goto mul;
	}
	auto mul_time = clock() - start;
	std::cout << "Multiplication: " << mul_time << std::endl;

	ResetArrays(length, a, b, ignoreit);
	ignoreit /= c[(rand() % (int)(length))];
	l = 0;
	start = clock();
div:
	for (int i = 0; i < length; ++i)
	{
		c[i] = a[i] / b[i];
	}
	while (l < loops)
	{
		++l;
		goto div;
	}
	auto div_time = clock() - start;
	ignoreit /= c[(rand() % (int)(length))];
	ignoreit += a[(rand() % (int)(length))] + b[(rand() % (int)(length))];
	std::cout << "Division: " << div_time << std::endl;

	std::cout << "Addition faster than multiplication: " << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl;
	std::cout << "Just ignore it: " << ignoreit << std::endl << std::endl;
}

void MulVsDiv::CsabaDynamicArrayLoop()
{
	const int length = 256;//1024 *1024* 8;
	const int loops = 1e6;//1e3 / 2;
	std::cout << "Csaba loop:\n---" << std::endl;
	std::vector<double> av(length), bv(length);
	double *a = &av.front(), *b = &bv.front();
	auto ignoreit = 0.0;

	ResetArrays(length, a, b, ignoreit);
	auto idx1 = (rand() % static_cast<int>(length)), idx2 = (rand() % static_cast<int>(length-1));
	auto start = clock();
	for (size_t l = 0; l < loops; l++)
	{
		// MSVC cannot vectorize this loop (message 1300).
		// However, if this loop will not be nested in, autovectorization will happen.
		// Same condition apply for mul/div/rcp loops

		// ICL does not have this issue, but to provide both vectorized and nonvectorized comparison 
		// i specifically disabled vectorization in this method
#pragma novector
#pragma loop( no_vector )
		for (int i = 1; i < length-1; ++i)
		{
			a[i] = b[idx1] + b[idx2];
		}
	}
	auto add_time = clock() - start;
	std::cout << "Addition: " << add_time << std::endl;

	ignoreit += a[(rand() % static_cast<int>(length))] + b[(rand() % static_cast<int>(length))];

	idx1 = (rand() % static_cast<int>(length)), idx2 = (rand() % static_cast<int>(length));

	start = clock();
	for (size_t l = 0; l < loops; l++)
	{
#pragma novector
#pragma loop( no_vector )
		for (int i = 0; i < length-1; ++i)
		{
			a[i] =b[idx1] * b[idx2];
		}
	}
	auto mul_time = clock() - start;
	std::cout << "Multiplication: " << mul_time << std::endl;

	ignoreit += a[(rand() % static_cast<int>(length))] + b[(rand() % static_cast<int>(length))];
	idx1 = (rand() % static_cast<int>(length)), idx2 = (rand() % static_cast<int>(length));
	start = clock();
	for (size_t l = 0; l < loops; l++)
	{
#pragma novector
#pragma loop( no_vector )
		for (int i = 0; i < length; ++i)
		{
			a[i] = b[idx1] / b[idx2];
		}
	}
	auto div_time = clock() - start;
	ignoreit += a[(rand() % static_cast<int>(length))] + b[(rand() % static_cast<int>(length))];
	std::cout << "Division: " << div_time << std::endl;
	std::cout << "Addition faster than multiplication: " << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division: " << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl;
	std::cout << "Just ignore it: " << ignoreit << std::endl << std::endl;
}

void MulVsDiv::DependendDynamicArrayLoop()
{
	const int length = 256;
	const int loops = 1e6;
	std::cout << "Dependend Dynamic loop:\n---" << std::endl;
	std::vector<double> av(length), bv(length);
	double *a = &av.front(), *b = &bv.front();
	
	std::vector<long> add_times;
	std::vector<long> mul_times;
	std::vector<long> div_times;
	add_times.reserve(loops);
	mul_times.reserve(loops);
	div_times.reserve(loops);
	unsigned long long ignoreit[]{0,0,0};
	
	for (size_t i = 0; i < length; i++)
	{
		av[i] = 8 * (static_cast<double>(rand()) / (RAND_MAX)) + 1;
		//bv[i] = DBL_MIN;
	}

	//Division
	for (size_t i = 0; i < loops; i++)
	{
		auto start = clock();
		for (size_t j = 0; j < length-1; j++)
		{
			b[j] = a[j + 1] / a[j];
		}
		auto finish = clock();
		div_times.push_back(finish-start);
		ignoreit[0] += std::accumulate(bv.begin(), bv.end(), 0);
	}
	auto div_time = std::accumulate(div_times.begin(), div_times.end(), 0)
		/ loops;
	std::cout << "Division:\t\t" << div_time << std::endl;
	//Multiplication
	for (size_t i = 0; i < loops; i++)
	{
		auto start = clock();
		for (size_t j = 0; j < length - 1; j++)
		{
			b[j] = a[j + 1] * a[j];
		}
		auto finish = clock();
		mul_times.push_back(finish - start);
		ignoreit[1] += std::accumulate(bv.begin(),bv.end(), 0);
	}
	auto mul_time = std::accumulate(mul_times.begin(), mul_times.end(), 0)
		/ loops;
	std::cout << "Multiplication:\t\t" << mul_time << std::endl;
	//Addition
	for (size_t i = 0; i < loops; i++)
	{
		auto start = clock();
		for (size_t j = 0; j < length - 1; j++)
		{
			b[j] = a[j + 1] + a[j];
		}
		auto finish = clock();
		add_times.push_back(finish - start);
		ignoreit[2] += std::accumulate(bv.begin(), bv.end(), 0);
	}
	auto add_time = std::accumulate(add_times.begin(), add_times.end(), 0)
		/ loops;
	std::cout << "Addition:\t\t" << add_time << std::endl;

	std::cout << "Addition faster than multiplication:\t" << static_cast<double>(mul_time) / static_cast<double>(add_time) << std::endl;
	std::cout << "Multiplication faster than division:\t" << static_cast<double>(div_time) / static_cast<double>(mul_time) << std::endl;
	std::cout << "Just ignore it: " << ignoreit[0]<< ignoreit[1]<< ignoreit[2] << std::endl << std::endl;
}


void MulVsDiv::BenchAll()
{
	Loop();
	//LoopVectorized();
	//DynamicArrayLoop();
	//DynamicArrayLoopVectorized();
	//DependendDynamicArrayLoop();
	//CsabaDynamicArrayLoop();
}

MulVsDiv::MulVsDiv()
{
}


MulVsDiv::~MulVsDiv()
{
}
