using System;
using System.Collections.Generic;

namespace Consola
{
    public interface IComando
    {
        void Ejecutar();
        void Deshacer();
    }

    public class AgendarCitaComando : IComando
    {
        private readonly Cita _cita;
        private readonly Salon _salon;

        public AgendarCitaComando(Cita cita, Salon salon)
        {
            _cita = cita;
            _salon = salon;
        }

        public void Ejecutar()
        {
            _salon.AgregarCita(_cita);
        }

        public void Deshacer()
        {
            _salon.CancelarCita(_cita);
        }
    }

    public class CancelarCitaComando : IComando
    {
        private readonly Cita _cita;
        private readonly Salon _salon;

        public CancelarCitaComando(Cita cita, Salon salon)
        {
            _cita = cita;
            _salon = salon;
        }

        public void Ejecutar()
        {
            _salon.CancelarCita(_cita);
        }

        public void Deshacer()
        {
            _salon.AgregarCita(_cita);
        }
    }

    public class ModificarCitaComando : IComando
    {
        private readonly Cita _citaAnterior;
        private readonly Cita _citaNueva;
        private readonly Salon _salon;

        public ModificarCitaComando(Cita citaAnterior, Cita citaNueva, Salon salon)
        {
            _citaAnterior = citaAnterior;
            _citaNueva = citaNueva;
            _salon = salon;
        }

        public void Ejecutar()
        {
            _salon.ModificarCita(_citaAnterior, _citaNueva);
        }

        public void Deshacer()
        {
            _salon.ModificarCita(_citaNueva, _citaAnterior);
        }
    }

    // Clase para representar una cita
    public class Cita
    {
        public DateTime Fecha { get; set; }
        public string NombreCliente { get; set; }
    }

    public class Salon
    {
        private readonly List<Cita> _citas;

        public Salon()
        {
            _citas = new List<Cita>();
        }

        public void AgregarCita(Cita cita)
        {
            _citas.Add(cita);
            Console.WriteLine($"Cita agendada para {cita.NombreCliente} el {cita.Fecha}");
        }

        public void CancelarCita(Cita cita)
        {
            _citas.Remove(cita);
            Console.WriteLine($"Cita cancelada para {cita.NombreCliente} el {cita.Fecha}");
        }

        public void ModificarCita(Cita citaAnterior, Cita citaNueva)
        {
            int index = _citas.IndexOf(citaAnterior);
            if (index != -1)
            {
                _citas.RemoveAt(index);
                _citas.Insert(index, citaNueva);
                Console.WriteLine($"Cita modificada de {citaAnterior.NombreCliente} el { citaAnterior.Fecha} a { citaNueva.NombreCliente} el { citaNueva.Fecha}");
            }
            else
            {
                Console.WriteLine("La cita a modificar no existe.");
            }
        }

        public void MostrarCitas()
        {
            Console.WriteLine("Citas agendadas:");
            for (int i = 0; i < _citas.Count; i++)
            {
                Console.WriteLine($"{i + 1}. { _citas[i].NombreCliente} el {_citas[i].Fecha}");
            }
        }

        public List<Cita> Citas => _citas;
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Creamos el salón de belleza de uñas
            Salon salon = new Salon();

            // Creamos una pila para almacenar los que se van ejecutando
            Stack<IComando> historialComandos = new Stack<IComando>();

            while (true)
            {
                Console.WriteLine("¿Qué acción desea realizar?");
                Console.WriteLine("1. Agendar una cita");
                Console.WriteLine("2. Modificar una cita");
                Console.WriteLine("3. Cancelar una cita");
                Console.WriteLine("4. Deshacer última acción");
                Console.WriteLine("5. Salir");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        Console.WriteLine("Ingrese el nombre del cliente:");
                        string nombreCliente = Console.ReadLine();
                        Console.WriteLine("Ingrese la fecha de la cita (formato: dd/mm/aaaa):");
                        DateTime fechaCita;
                        while (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out fechaCita))
                        {
                            Console.WriteLine("Formato de fecha inválido. Intente nuevamente (formato: dd/mm/aaaa):");
                        }
                        Cita cita = new Cita { Fecha = fechaCita, NombreCliente = nombreCliente };
                        IComando agendarComando = new AgendarCitaComando(cita, salon);
                        agendarComando.Ejecutar();
                        historialComandos.Push(agendarComando);
                        break;
                    case "2":
                        if (salon.Citas.Count == 0)
                        {
                            Console.WriteLine("No hay citas agendadas para modificar.");
                            break;
                        }
                        salon.MostrarCitas();
                        Console.WriteLine("Seleccione el número de cita que desea modificar:");
                        int indexModificar;
                        while (!int.TryParse(Console.ReadLine(), out indexModificar) || indexModificar < 1 || indexModificar > salon.Citas.Count)
                        {
                            Console.WriteLine("Selección inválida. Intente nuevamente:");
                        }
                        Cita citaAnterior = salon.Citas[indexModificar - 1];
                        Console.WriteLine("Ingrese el nuevo nombre del cliente:");
                        string nuevoNombreCliente = Console.ReadLine();
                        Console.WriteLine("Ingrese la nueva fecha de la cita (formato: dd/mm/aaaa):");
                        DateTime nuevaFechaCita;
                        while (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out nuevaFechaCita))
                        {
                            Console.WriteLine("Formato de fecha inválido. Intente nuevamente (formato: dd/mm/aaaa):");
                        }
                        Cita citaNueva = new Cita { Fecha = nuevaFechaCita, NombreCliente = nuevoNombreCliente };
                        IComando modificarComando = new ModificarCitaComando(citaAnterior, citaNueva, salon);
                        modificarComando.Ejecutar();
                        historialComandos.Push(modificarComando);
                        break;
                    case "3":
                        if (salon.Citas.Count == 0)
                        {
                            Console.WriteLine("No hay citas agendadas para cancelar.");
                            break;
                        }
                        salon.MostrarCitas();
                        Console.WriteLine("Seleccione el número de cita que desea cancelar:");
                        int index;
                        while (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > salon.Citas.Count)
                        {
                            Console.WriteLine("Selección inválida. Intente nuevamente:");
                        }
                        IComando cancelarComando = new CancelarCitaComando(salon.Citas[index - 1], salon);
                        cancelarComando.Ejecutar();
                        historialComandos.Push(cancelarComando);
                        break;
                    case "4":
                        if (historialComandos.Count == 0)
                        {
                            Console.WriteLine("No hay acciones para deshacer.");
                            break;
                        }
                        IComando ultimoComando = historialComandos.Pop();
                        ultimoComando.Deshacer();
                        break;
                    default:
                        Console.WriteLine("Opción inválida. Intente nuevamente.");
                        break;
                }
            }
        }
    }
}