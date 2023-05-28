namespace TaskBoardApp.Data
{
	public class DataConstants
	{
		public class User
		{
			public const int UserFirstNameMaxLength = 15;
			public const int UserLastNameMaxLength = 15;
		}

		public class Task
		{
			public const int TaskTitleMaxLength = 70;
			public const int TaskTitleMinLength = 5;

			public const int TaskDescriptionMaxLength = 1000;
			public const int TaskDescriptionMinLength = 10;
		}

		public class Board
		{
			public const int BoardNameMaxLength = 30;
			public const int BoardNameMinLenght = 3;
		}
	}
}
