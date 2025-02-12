using System;

namespace Code_Dictionary_C_
{
    // Interface---------------------------------------------
    //=======================================================
    interface Ianimal
    {
        string Name { get; set; }
        void Eat();
        void Run();
    }
    //=======================================================

    // Class-------------------------------------------------
    //=======================================================
    class Person : Ianimal
    {
        public string Name { get; set; }

        public void Eat()
        {
            Console.WriteLine("밥을 먹습니다.");
        }

        public void Run()
        {
            Console.WriteLine("두 발로 달립니다. .");
        }
    }

    class Dog : Ianimal
    {
        public string Name { get; set; }

        public void Eat()
        {
            Console.WriteLine("사료를 먹습니다.");
        }

        public void Run()
        {
            Console.WriteLine("네 발로 달립니다. .");
        }
    }


    class Lion : Ianimal
    {
        public string Name { get; set; }

        public void Eat()
        {
            Console.WriteLine("고기를 먹습니다.");
        }

        public void Run()
        {
            Console.WriteLine("네 발로 달립니다. .");
        }
    }

    class cat : Ianimal
    {
        public string Name { get; set; }

        public void Eat()
        {
            Console.WriteLine("사료를 먹습니다.");
        }

        public void Run()
        {
            Console.WriteLine("네 발로 달립니다. .");
        }
    }
    //=======================================================


    public class InterfaceClass
    {
        // 솔루션-1
        Ianimal ianimal_Person = new Person();
        Ianimal ianimal_Dog = new Dog();
        Ianimal ianimal_Lion = new Lion();
        Ianimal ianimal_Cat = new cat();


        // 솔루션-2
        Person person = new Person();
        cat cat = new cat();
        Lion lion = new Lion();

        public InterfaceClass()
        {
            // 솔루션-1
            ianimal_Person.Eat();
            ianimal_Dog.Eat();
            ianimal_Lion.Run();
            ianimal_Cat.Run();

            // 솔루션-2
            HowToEatAnimals(person);
            HowToEatAnimals(lion);
            HowToEatAnimals(cat);
            HowToRunAnimals(person);
            HowToRunAnimals(lion);
            HowToRunAnimals(cat);

        }

        // 솔루션-2
        private void HowToEatAnimals(object obj)
        {
            Ianimal target = obj as Ianimal;
            //bool afsf = obj is Ianimal;
            //Ianimal2 target_ = obj as Ianimal2;
            target.Eat();
            //target_.Run();
        }

        private void HowToRunAnimals(object obj)
        {
            Ianimal Target = obj as Ianimal;

            Target.Run();
        }


    }
}
