using System;

namespace ReqAssessment1
{
    /// <summary>
    /// This program calculates the maximum height of a shell 
    /// and the distance it will travel along the ground.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main routine for the program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // print some infos to the user
            Console.WriteLine("Welcome!"); 
            Console.WriteLine("This program will calculate the maximum height of a shell and"); 
            Console.WriteLine("the distance it will travel along the ground.");
            Console.WriteLine();

            // constant acceleration due to gravity
            const float GRAVITY_ACCELERATION = 9.8f;

            // ask the user for the initial angle and transform the angle into radians
            Console.WriteLine("Please give the initial firing angle in degrees:");
            float theta = float.Parse(Console.ReadLine());
            Console.WriteLine();
            theta = (float) (Math.PI * theta / 180.0);

            // ask the user for the initial speed
            Console.WriteLine("Please give the initial firing speed:");
            float speed = float.Parse(Console.ReadLine());
            Console.WriteLine();

            // calculate velocity, t, h, dx
            float velocityX = (float)(speed * Math.Cos(theta));
            float velocityY = (float)(speed * Math.Sin(theta));
            float time = velocityY / GRAVITY_ACCELERATION;
            float height = velocityY * velocityY / (2 * GRAVITY_ACCELERATION);
            float distance = velocityX * 2 * time;
 
            // print the results
            Console.WriteLine("Maximum shell height: " + height.ToString("F3")  + " meters");
            Console.WriteLine("Horizontal distance: " + distance.ToString("F3") + " meters");
            Console.WriteLine();

        }
    }
}
