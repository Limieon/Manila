#include <iostream>
#include <filesystem>

#include "Math.hpp"

int main(int argc, char** argv) {
	std::cout << "Working directory: " << std::filesystem::current_path() << '\n';

	std::cout << "Arguments:" << std::endl;
	for(int i = 1; i < argc; ++i) {
		std::cout << "  " << argv[i] << std::endl;
	}

	std::cout << "Starting Client..." << std::endl;
	std::cout << "7 + 9 = " << Math::add(7, 9) << std::endl;
	return 0;
}
