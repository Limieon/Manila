#include <iostream>
#include "Math.hpp"

int main(int argc, char** argv) {
	std::cout << "Hello Client!" << std::endl;
	std::cout << "9 + 3 = " << manila::Math::add(9, 3) << std::endl;
	std::cout << "7 - 4 = " << manila::Math::sub(7, 4) << std::endl;
}
