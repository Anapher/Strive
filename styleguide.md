## Test Method Naming
Test Methods are named after https://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html

Naming: [UnitOfWork_StateUnderTest_ExpectedBehavior]

Examples:

Public void Sum_NegativeNumberAs1stParam_ExceptionThrown()
Public void Sum_NegativeNumberAs2ndParam_ExceptionThrown()
Public void Sum_simpleValues_Calculated()

## Exceptions vs Return Codes
There is an ongoing discussion whether exceptions or return codes are a better design choice.
Robert C. Martin says in his book "Clean Code", that you should always prefer exceptions, because
- they cannot be accidentally ignored 
- try/catch blocks are easier to read then lots of if statements
- methods with no return code are easier chainable

I still decided that Return Codes are the better design choice for this project in most cases, as:
- they are more lightweight
- especially for errors that are expected (validation/permission issues), exceptions just don't sound right. Exceptions are only used for actually exceptional behavior that should normally not occurre/is not expected
- For nice responses, we need a common error format with error codes etc. Using Return codes, that is much easier to implement.

For all top level code (especially UseCases), we use a return code (using `SuccessOrError`) for expected/user errors and exceptions if something very unexpected happend.

For low level code please still use exceptions or out parameters and a return boolean (like in `Dictionary.TryGetValue()`).