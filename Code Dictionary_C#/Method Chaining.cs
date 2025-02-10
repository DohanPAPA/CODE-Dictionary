namespace Code_Dictionary_C_
{
    // 메서드 체이닝 
    // 여러 메소드 호출을 단일 명령문의 형태로 결합해서 일련의 작업을 수행할 수 있는 기술
    internal class Method_Chaining
    {
        Calculator calc = new Calculator(5);

        public Method_Chaining()
        {
            // 연쇄적으로 메서드 호출 가능
            calc.Add(2).Subtract(2).Multiply(2);
        }
    }

    public class Calculator
    {
        private int _value;

        public Calculator(int initiaValue)
        {
            _value = initiaValue;
        }

        public Calculator Add(int number)
        {
            _value += number;
            return this;
        }
        public Calculator Subtract(int number)
        {
            _value -= number;
            return this;
        }
        public Calculator Multiply(int number)
        {
            _value *= number;
            return this;
        }
    }

}
