using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Data
{
    public class Component
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public String Title { get; set; } = String.Empty;
        public String Description { get; set; } = String.Empty;
        public EDataType DataType { get; set; }
        public ECategoryType CategoryType { get; set; }
        // Если понадобится делать ограничения для типов jsonb
        // public DataTypeMeta DataTypeMeta { get; set; }
        public Guid ComponentCategoryId { get; set; }
        public ComponentCategory ComponentCategory { get; set; } = null!;
        
        public ICollection<ProductComponent> ProductComponents
        { get; set; } = new List<ProductComponent>();
    }

    public enum EDataType
    {
        STRING,
        INT,
        DOUBLE,
        BOOLEAN
    }

    public enum ECategoryType
    {
        // Основные характеристики
        // ОС
        MAIN,
        // Дисплей
        // Диагональ экрана
        // Матрица
        // Тип дисплея (Ємнісний)
        // Плотность пикселей (PPI)
        DISPLAY,
        // Оперативная память
        // Встроенная память
        //Слот для карты памяти
        MAMORY,
        // Процессор
        // Кол-во ядер
        // Графический процессор
        PROCESSOR,
        // Тип sim-карт
        FUNCTIONAL,
        // 2g, 3g, 4g, 5g
        // NFC, Bluetooth, Wi-Fi
        // GPS, ГЛОНАСС, Galileo, BeiDou
        COMUNICATION,
        // Основна камера
        // Діафрагма
        // Спалах
        // Фокусування
        // Фронтальна камера 
        // Спалах фронтальної камери 
        CAMERA,
        // Аудіо
        // Мікрофон 
        // Динаміки (стерео, моно)
        AUDIO,
        // Порты и разъемы
        INTERFACES,
        // Корпус тлефона
        // Матеріал корпусу 
        // Колір
        CASE,
        // Акумулятор
        // Ємність аккумулятору, мАгод 
        // Особливості швидкої зарядки
        BATTERY,   
        OTHER
    }
}
