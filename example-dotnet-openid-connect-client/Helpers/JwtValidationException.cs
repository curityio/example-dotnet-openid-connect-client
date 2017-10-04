using System;

public class JwtValidationException : Exception
{
	public JwtValidationException()
	{
	}

	public JwtValidationException(string message)
		: base(message)
	{
	}

	public JwtValidationException(string message, Exception inner)
		: base(message, inner)
	{
	}
}