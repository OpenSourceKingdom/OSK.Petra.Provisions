# OSK.Petra.Provisions

Provides a library that handles provision expense and usage calcuations and validations, as well as refund/recovery from previous provision calculations. This library is useful for a shared
foundation for economy or similar style application requirement. 

## Usage

Developers can access the primary functions of the library by using:
* `ProvisionCalculator`: calculates an expenditure and recovery summary, given a set of provisions and adjustments. This is essential to handling economic related logic
* `IProvisionSet`: represents a fundamental set of provisions. Includes provision management related logic and extensions to hook into the calculator. This data structure can manage economy related math and data storage
