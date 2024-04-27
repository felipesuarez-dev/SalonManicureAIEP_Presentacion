using System;
using System.Collections.Generic;
using System.Threading;

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
            Console.WriteLine($"\nCita agendada para {cita.NombreCliente} el {cita.Fecha}\n\n");
        }

        public void CancelarCita(Cita cita)
        {
            _citas.Remove(cita);
            Console.WriteLine($"\nCita cancelada para {cita.NombreCliente} el {cita.Fecha}\n\n");
        }

        public void ModificarCita(Cita citaAnterior, Cita citaNueva)
        {
            int index = _citas.IndexOf(citaAnterior);
            if (index != -1)
            {
                _citas.RemoveAt(index);
                _citas.Insert(index, citaNueva);
                Console.WriteLine($"\nCita modificada de {citaAnterior.NombreCliente} el { citaAnterior.Fecha} a { citaNueva.NombreCliente} el { citaNueva.Fecha}\n\n");
            }
            else
            {
                Console.WriteLine("\nLa cita a modificar no existe.\n\n");
            }
        }

        public void MostrarCitas()
        {
            Console.WriteLine("\nCitas agendadas:\n");
            for (int i = 0; i < _citas.Count; i++)
            {
                Console.WriteLine($"{i + 1}. { _citas[i].NombreCliente} el {_citas[i].Fecha}\n\n");
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
                Console.WriteLine("***********************************************************************************************************");
                Console.WriteLine("   _____           _     __              __  __                   _                              ");
                Console.WriteLine("  / ____|         | |   /_/             |  \\/  |                 (_)                             ");
                Console.WriteLine(" | (___     __ _  | |   ___    _ __     | \\  / |   __ _   _ __    _    ___   _   _   _ __    ___ ");
                Console.WriteLine("  \\___ \\   / _` | | |  / _ \\  | '_ \\    | |\\/| |  / _` | | '_ \\  | |  / __| | | | | | '__|  / _ \\");
                Console.WriteLine("  ____) | | (_| | | | | (_) | | | | |   | |  | | | (_| | | | | | | | | (__  | |_| | | |    |  __/");
                Console.WriteLine(" |_____/   \\__,_| |_|  \\___/  |_| |_|   |_|  |_|  \\__,_| |_| |_| |_|  \\___|  \\__,_| |_|     \\___|");
                Console.WriteLine("                                                                                                 ");
                Console.WriteLine("                                                                                                 ");
                Console.WriteLine("**********************************************************************************************************");
                Console.WriteLine("\nMENÚ\n");
                Console.WriteLine("1. Agendar una cita");
                Console.WriteLine("2. Modificar una cita");
                Console.WriteLine("3. Cancelar una cita");
                Console.WriteLine("4. Deshacer última acción");
                Console.WriteLine("5. Salir\n");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        Console.WriteLine("\nAGENDAR CITA\n");
                        Console.WriteLine("\nIngrese el nombre del cliente:");
                        string nombreCliente = Console.ReadLine();
                        Console.WriteLine("\nIngrese la fecha de la cita (formato: dd/mm/aaaa):");
                        DateTime fechaCita;
                        while (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out fechaCita))
                        {
                            Console.WriteLine("\nFormato de fecha inválido. Intente nuevamente (formato: dd/mm/aaaa):");
                        }
                        Cita cita = new Cita { Fecha = fechaCita, NombreCliente = nombreCliente };
                        IComando agendarComando = new AgendarCitaComando(cita, salon);
                        agendarComando.Ejecutar();
                        historialComandos.Push(agendarComando);
                        break;
                    case "2":
                        Console.WriteLine("\nMODIFICAR CITA\n");
                        if (salon.Citas.Count == 0)
                        {
                            Console.WriteLine("\nNo hay citas agendadas para modificar.");
                            break;
                        }
                        salon.MostrarCitas();
                        Console.WriteLine("\nSeleccione el número de cita que desea modificar:");
                        int indexModificar;
                        while (!int.TryParse(Console.ReadLine(), out indexModificar) || indexModificar < 1 || indexModificar > salon.Citas.Count)
                        {
                            Console.WriteLine("\nSelección inválida. Intente nuevamente:");
                        }
                        Cita citaAnterior = salon.Citas[indexModificar - 1];
                        Console.WriteLine("\nIngrese el nuevo nombre del cliente:");
                        string nuevoNombreCliente = Console.ReadLine();
                        Console.WriteLine("\nIngrese la nueva fecha de la cita (formato: dd/mm/aaaa):");
                        DateTime nuevaFechaCita;
                        while (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out nuevaFechaCita))
                        {
                            Console.WriteLine("\nFormato de fecha inválido. Intente nuevamente (formato: dd/mm/aaaa):");
                        }
                        Cita citaNueva = new Cita { Fecha = nuevaFechaCita, NombreCliente = nuevoNombreCliente };
                        IComando modificarComando = new ModificarCitaComando(citaAnterior, citaNueva, salon);
                        modificarComando.Ejecutar();
                        historialComandos.Push(modificarComando);
                        break;
                    case "3":
                        Console.WriteLine("\nCANCELAR CITA\n");
                        if (salon.Citas.Count == 0)
                        {
                            Console.WriteLine("\nNo hay citas agendadas para cancelar.");
                            break;
                        }
                        salon.MostrarCitas();
                        Console.WriteLine("\nSeleccione el número de cita que desea cancelar:");
                        int index;
                        while (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > salon.Citas.Count)
                        {
                            Console.WriteLine("\nSelección inválida. Intente nuevamente:");
                        }
                        IComando cancelarComando = new CancelarCitaComando(salon.Citas[index - 1], salon);
                        cancelarComando.Ejecutar();
                        historialComandos.Push(cancelarComando);
                        break;
                    case "4":
                        if (historialComandos.Count == 0)
                        {
                            Console.WriteLine("\nNo hay acciones para deshacer.");
                            break;
                        }
                        IComando ultimoComando = historialComandos.Pop();
                        ultimoComando.Deshacer();
                        break;
                    case "5":
                        Console.WriteLine("\n¡Gracias por agendar con nosotros!");
                        SalirPrograma(5);                        
                        break;
                    default:
                        Console.WriteLine("\nOpción inválida. Intente nuevamente.\n\n\n");
                        break;
                }
            }
        }

        static void SalirPrograma(int seconds)
        {
            Console.Write("Saliendo del programa");
            for (int i = 0; i < seconds; i++)
            {
                Console.Write(".");
                Thread.Sleep(1000);
            }
            Console.WriteLine();
            Environment.Exit(0);
        }
    }
}