#include <iostream>

#include "Math.hpp"

int main(int argc, char** argv) {
	std::cout << "Starting Client..." << std::endl;
	std::cout << "7 + 9 = " << Math::add(7, 9) << std::endl;
	return 0;
}
