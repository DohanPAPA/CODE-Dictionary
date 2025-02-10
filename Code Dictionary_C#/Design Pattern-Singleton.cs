using System;

namespace Code_Dictionary_C_
{
    // 디자인 패턴의 종류
    // 1. 생성 패턴 : 팩토리 메서드, 추상 팩토리, 빌더, 프로토타입, 싱글톤
    // 2. 구조 패턴 : 어댑터, 브리지, 복합체, 데코레이터, 퍼사드, 플라이웨이트, 프록시
    // 3. 행동 패턴 : 책임 연쇄 , 커맨드, 반복자, 중재자, 메멘토, 옵서버, 상태, 전략, 템플릿 메서드, 비지터



    // 싱글톤 패턴이란?
    // 싱글톤 패턴은 특정 클래스의 인스턴스를 1개만 생성되는 것을 보장하는 디자인 패턴이다.
    // 즉, 생성자를 통해서 여러 번 호출이 되더라도 인스턴스를 새로 생성하지 않고 최초 호출 시에 만들어두었던 인스턴스를 재활용하는 패턴이다.

    // 싱글톤 패턴은 런타임 동안 단 하나의 인스턴스만을 생서하는 패턴을 의미한다. 
    // 싱글톤 패턴이 적용된 객체는 하나의 인스턴스만 생성할 수 있으며 다른 객체에서 싱글톤 객체의 인스턴스를 생성하려 할 경우 기존에 생성된 인스턴스가 있다면
    // 해당 인스턴스를 반환하는 형태이다.

    // 장점
    // 1. 메모리 절약과 성능향상
    // 2. 데이터 관리의 측면에서 유리
    // 단점
    // 의존성이 높아진다. -> 객체지향적으로 설계가 힘듬
    // private 생성자 때문에 상속이 어렵다.
    // 테스트하기 힘들다. 안티패턴이라고 하기도 함

    internal class SingletonClass
    {
        private static SingletonClass _instance;

        public static SingletonClass Instance
        {
            get
            {
                // 생성된 인스턴스가 없으면 생성합니다.
                if (_instance == null)
                {
                    _instance = new SingletonClass();
                }

                return _instance;
            }
        }

        // 생성자
        public SingletonClass()
        {

        }
    }


    // Lazy 싱글톤 적용
    //인스턴스의 생성 시기를 선언 즉시 생성이 아니라 인스턴스 내에 있는 값을 접근하려 할 때 생성시켜주는 방법으로 보다 효율적으로 사용할 수 있다.
    //또한 일반적인 싱글톤에서는 thread-safety를 추가적으로 고려해야 하지만 Lazy 싱글톤을 사용하면 이를 고려하지 않아도 보장된다는 장점이 있다.
    //(Lazy 싱글톤을 이용한 thread-safety 보장은 인스턴스의 생성에 대한 thread-safety를 보장하는 것이지 이미 생성된 인스턴스의 접근에 대한 보장이 아님.)

    public class SingletonLazy<T> where T : SingletonLazy<T>, new() //제너릭 형식 제약조건(옵션)
    {
        private static Lazy<T> lazyInstance = null;

        public static T Instance
        {
            get
            {
                if (Exists() == false)
                {
                    var instance = new T();
                    lazyInstance = new Lazy<T>(() => instance);
                }

                return lazyInstance.Value;
            }
        }
        //인스턴스가 만들어졌는지 체크합니다.
        public static bool Exists()
        {
            return lazyInstance != null && lazyInstance.IsValueCreated;
        }
        //인스턴스 생성이력을 초기화 할때 사용합니다.
        public static void ClearInstance()
        {
            lazyInstance = null;
        }
    }
}
