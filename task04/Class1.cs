namespace task04;

public interface ISpaceship
{
    void MoveForward();      // Движение вперед
    void Rotate(int angle);  // Поворот на угол (градусы)
    void Fire();             // Выстрел ракетой
    int Speed { get; }       // Скорость корабля
    int FirePower { get; }   // Мощность выстрела
}

public class Cruiser : ISpaceship
{
    public int Speed { get; } = 50;
    public int FirePower { get; } = 100;

    public void MoveForward()
    {
        System.Console.WriteLine($"Крейсер движется впред! его скорость: {Speed}");
    }
    public void Rotate(int angle)
    {
        System.Console.WriteLine($"Крейсер поворачивается на {angle} градусов!");
    }
    public void Fire()
    {
        System.Console.WriteLine($"Крейсер атакует! мощность его выстрела: {FirePower}");
    }

}
public class Fighter : ISpaceship
{
    public int Speed { get;  } = 100;
    public int FirePower { get;  } = 50;

    public void MoveForward()
    {
        System.Console.WriteLine($"Истребитель мчиться впред! его скорость: {Speed}");
    }
    public void Rotate(int angle)
    {
        System.Console.WriteLine($"Истребитель поворачивается на {angle} градусов!");
    }
    public void Fire()
    {
        System.Console.WriteLine($"Истребитель атакует! мощность его выстрела: {FirePower}");
    }

}