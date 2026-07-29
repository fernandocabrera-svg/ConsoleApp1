using System;

namespace SistemaGestionUnicaes
{
    struct Estudiante
    {
        public string Codigo;
        public string Carnet;
        public string Nombre;
        public string Genero;
        public int Edad;
        public string Facultad;
        public string Carrera;
        public string Materia;
        public double[] Notas;
        public double Asistencia;
        public bool Activo;
    }

    internal class Program
    {
        const int MAX_ESTUDIANTES_POR_CARRERA = 50;
        const int MAX_CARRERAS = 4;
        const int CAPACIDAD = MAX_ESTUDIANTES_POR_CARRERA * MAX_CARRERAS;

        const int CANT_NOTAS = 3;

        const double NOTA_MINIMA_APROBACION = 6.0;
        const double ASISTENCIA_MINIMA_APROBACION = 75.0;
        const int EDAD_MINIMA = 16;
        const int EDAD_MAXIMA = 50;

        static string[] LISTA_FACULTADES = {
            "Facultad de Ingenieria y Arquitectura",
            "Facultad de Ciencias de la Salud",
            "Facultad de Ciencias y Humanidades"
        };

        static string[] LISTA_CARRERAS = {
            "Ingenieria en Sistemas Informaticos",
            "Ingenieria Industrial",
            "Doctorado en Medicina",
            "Licenciatura en Psicologia"
        };

        static string[] FACULTAD_DE_CARRERA = {
            "Facultad de Ingenieria y Arquitectura",
            "Facultad de Ingenieria y Arquitectura",
            "Facultad de Ciencias de la Salud",
            "Facultad de Ciencias y Humanidades"
        };

        static string[,] MATERIAS_POR_CARRERA = {
            { "Programacion I", "Base de Datos I", "Calculo I", "Redes de Computadoras", "Fisica I" },
            { "Gestion de Calidad", "Investigacion de Operaciones", "Estadistica", "Procesos Industriales", "Ergonomia" },
            { "Anatomia Humana", "Biologia Celular", "Bioquimica", "Fisiologia", "Farmacologia" },
            { "Psicologia General", "Neuroanatomia", "Psicologia del Desarrollo", "Psicofisiologia", "Teorias de la Personalidad" }
        };

        static Estudiante[] listaEstudiantes = new Estudiante[CAPACIDAD];

        static void Main(string[] args)
        {
            int opcion = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("=========================================================");
                Console.WriteLine("  UNICAES - SISTEMA DE GESTION ACADEMICA (CON STRUCT)    ");
                Console.WriteLine("=========================================================");
                Console.WriteLine("1. Registrar Estudiante");
                Console.WriteLine("2. Consultar / Mostrar Informacion de Estudiantes");
                Console.WriteLine("3. Buscar Estudiante (por Carnet o Codigo)");
                Console.WriteLine("4. Modificar Registro (Notas y Asistencia)");
                Console.WriteLine("5. Eliminar Estudiante");
                Console.WriteLine("6. Estadisticas Generales por Facultad");
                Console.WriteLine("7. Estadisticas Generales por Carrera");
                Console.WriteLine("8. Reporte Detallado Seleccionable por Carrera");
                Console.WriteLine("9. Salir");
                Console.WriteLine("=========================================================");
                Console.Write("Seleccione una opcion: ");

                if (!int.TryParse(Console.ReadLine(), out opcion)) opcion = 0;

                Console.Clear();
                switch (opcion)
                {
                    case 1: Registrar(); break;
                    case 2: Mostrar(); break;
                    case 3: Buscar(); break;
                    case 4: Modificar(); break;
                    case 5: Eliminar(); break;
                    case 6: EstadisticasFacultad(); break;
                    case 7: EstadisticasCarrera(); break;
                    case 8: ReporteCarrera(); break;
                    case 9: Console.WriteLine("Saliendo del sistema..."); break;
                    default: Console.WriteLine("Opcion invalida."); break;
                }

                if (opcion != 9)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }

            } while (opcion != 9);
        }

        static void Registrar()
        {
            Console.WriteLine("REGISTRO DE ESTUDIANTE");
            int idxLibre = -1;
            for (int i = 0; i < CAPACIDAD; i++)
            {
                if (!listaEstudiantes[i].Activo) { idxLibre = i; break; }
            }

            if (idxLibre == -1)
            {
                Console.WriteLine("Error: Se ha alcanzado la capacidad maxima del sistema.");
                return;
            }

            Console.WriteLine("Seleccione la Carrera:");
            for (int i = 0; i < LISTA_CARRERAS.Length; i++)
            {
                Console.WriteLine((i + 1) + ". " + LISTA_CARRERAS[i]);
            }
            Console.Write("Opcion: ");
            int cSel;
            while (!int.TryParse(Console.ReadLine(), out cSel) || cSel < 1 || cSel > LISTA_CARRERAS.Length)
            {
                Console.Write("Opcion invalida. Reintente: ");
            }

            int idxCarrera = cSel - 1;
            string carreraStr = LISTA_CARRERAS[idxCarrera];

            int cInscritos = 0;
            for (int i = 0; i < CAPACIDAD; i++)
            {
                if (listaEstudiantes[i].Activo && listaEstudiantes[i].Carrera == carreraStr) cInscritos++;
            }

            if (cInscritos >= MAX_ESTUDIANTES_POR_CARRERA)
            {
                Console.WriteLine("[ERROR] La carrera '" + carreraStr + "' ya alcanzo el limite maximo de " + MAX_ESTUDIANTES_POR_CARRERA + " estudiantes.");
                return;
            }

            Console.WriteLine("Seleccione la Materia para esta carrera:");
            for (int j = 0; j < 5; j++)
            {
                Console.WriteLine((j + 1) + ". " + MATERIAS_POR_CARRERA[idxCarrera, j]);
            }
            Console.Write("Opcion: ");
            int mSel;
            while (!int.TryParse(Console.ReadLine(), out mSel) || mSel < 1 || mSel > 5)
            {
                Console.Write("Opcion invalida. Reintente: ");
            }
            string materiaStr = MATERIAS_POR_CARRERA[idxCarrera, mSel - 1];

            Estudiante est = new Estudiante();
            est.Carrera = carreraStr;
            est.Facultad = FACULTAD_DE_CARRERA[idxCarrera];
            est.Materia = materiaStr;
            est.Notas = new double[CANT_NOTAS];

            do
            {
                Console.Write("Codigo Interno Unico: ");
                est.Codigo = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrEmpty(est.Codigo)) Console.WriteLine("El codigo no puede estar vacio.");
                else if (ExisteCod(est.Codigo)) Console.WriteLine("Este codigo ya existe.");
            } while (string.IsNullOrEmpty(est.Codigo) || ExisteCod(est.Codigo));

            do
            {
                Console.Write("Carnet del Estudiante: ");
                est.Carnet = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrEmpty(est.Carnet)) Console.WriteLine("El carnet no puede estar vacio.");
                else if (ExisteCar(est.Carnet)) Console.WriteLine("Este carnet ya existe.");
            } while (string.IsNullOrEmpty(est.Carnet) || ExisteCar(est.Carnet));

            Console.Write("Nombre Completo: ");
            est.Nombre = Console.ReadLine()?.Trim() ?? "Sin Nombre";

            string g;
            do
            {
                Console.Write("Genero (M = Masculino, F = Femenino): ");
                g = Console.ReadLine()?.Trim().ToUpper() ?? "";
            } while (g != "M" && g != "F");
            est.Genero = (g == "M") ? "Masculino" : "Femenino";

            do
            {
                Console.Write("Edad (" + EDAD_MINIMA + " a " + EDAD_MAXIMA + " años): ");
            } while (!int.TryParse(Console.ReadLine(), out est.Edad) || est.Edad < EDAD_MINIMA || est.Edad > EDAD_MAXIMA);

            Console.WriteLine("Ingreso de las " + CANT_NOTAS + " Evaluaciones:");
            for (int k = 0; k < CANT_NOTAS; k++)
            {
                est.Notas[k] = LeerNota("Nota " + (k + 1) + " (0 a 10): ");
            }

            do
            {
                Console.Write("Porcentaje de Asistencia (0 a 100%): ");
            } while (!double.TryParse(Console.ReadLine(), out est.Asistencia) || est.Asistencia < 0 || est.Asistencia > 100);

            est.Activo = true;
            listaEstudiantes[idxLibre] = est;

            double cumCalculado = CalcularCUM(est);
            Console.WriteLine("[EXITO] Estudiante registrado correctamente.");
            Console.WriteLine("CUM del Estudiante: " + cumCalculado.ToString("F2"));
        }

        static void Mostrar()
        {
            Console.WriteLine("LISTADO DE ESTUDIANTES REGISTRADOS");
            bool alguno = false;
            for (int i = 0; i < CAPACIDAD; i++)
            {
                if (listaEstudiantes[i].Activo)
                {
                    alguno = true;
                    Estudiante e = listaEstudiantes[i];
                    double cum = CalcularCUM(e);

                    Console.WriteLine();
                    Console.WriteLine("Codigo: " + e.Codigo + " | Carnet: " + e.Carnet);
                    Console.WriteLine("Nombre: " + e.Nombre + " | Genero: " + e.Genero + " | Edad: " + e.Edad);
                    Console.WriteLine("Facultad: " + e.Facultad);
                    Console.WriteLine("Carrera: " + e.Carrera + " | Materia: " + e.Materia);
                    
                    Console.Write("Notas: ");
                    for (int k = 0; k < CANT_NOTAS; k++)
                    {
                        Console.Write("N" + (k + 1) + "=" + e.Notas[k].ToString("F2") + (k < CANT_NOTAS - 1 ? ", " : ""));
                    }
                    Console.WriteLine(" | CUM: " + cum.ToString("F2"));
                    Console.WriteLine("Asistencia: " + e.Asistencia.ToString("F2") + "%");
                }
            }
            if (!alguno) Console.WriteLine("No hay estudiantes activos en el sistema.");
        }

        static void Buscar()
        {
            Console.WriteLine("BUSCAR ESTUDIANTE");
            Console.Write("Ingrese el Carnet o Codigo a buscar: ");
            string q = Console.ReadLine()?.Trim() ?? "";
            int i = BuscarIdx(q);

            if (i != -1)
            {
                Estudiante e = listaEstudiantes[i];
                double cum = CalcularCUM(e);

                Console.WriteLine("ESTUDIANTE ENCONTRADO");
                Console.WriteLine("Codigo: " + e.Codigo + " | Carnet: " + e.Carnet);
                Console.WriteLine("Nombre: " + e.Nombre + " | Genero: " + e.Genero + " | Edad: " + e.Edad);
                Console.WriteLine("Facultad: " + e.Facultad);
                Console.WriteLine("Carrera: " + e.Carrera + " | Materia: " + e.Materia);
                
                Console.Write("Notas: ");
                for (int k = 0; k < CANT_NOTAS; k++)
                {
                    Console.Write("N" + (k + 1) + "=" + e.Notas[k].ToString("F2") + (k < CANT_NOTAS - 1 ? ", " : ""));
                }
                Console.WriteLine(" | CUM Final: " + cum.ToString("F2"));
                Console.WriteLine("Asistencia: " + e.Asistencia.ToString("F2") + "%");
            }
            else
            {
                Console.WriteLine("Estudiante no encontrado.");
            }
        }

        static void Modificar()
        {
            Console.WriteLine("MODIFICAR NOTAS Y ASISTENCIA");
            Console.Write("Ingrese el Carnet o Codigo del estudiante a editar: ");
            string q = Console.ReadLine()?.Trim() ?? "";
            int i = BuscarIdx(q);

            if (i != -1)
            {
                Console.WriteLine("Modificando registro de: " + listaEstudiantes[i].Nombre);

                for (int k = 0; k < CANT_NOTAS; k++)
                {
                    listaEstudiantes[i].Notas[k] = LeerNota("Nueva Nota " + (k + 1) + " (0 a 10): ");
                }

                double a;
                do
                {
                    Console.Write("Nuevo Porcentaje de Asistencia (0 a 100%): ");
                } while (!double.TryParse(Console.ReadLine(), out a) || a < 0 || a > 100);

                listaEstudiantes[i].Asistencia = a;

                double nuevoCum = CalcularCUM(listaEstudiantes[i]);
                Console.WriteLine("[EXITO] Registro actualizado correctamente.");
                Console.WriteLine("Nuevo CUM calculado: " + nuevoCum.ToString("F2"));
            }
            else
            {
                Console.WriteLine("Estudiante no encontrado.");
            }
        }

        static void Eliminar()
        {
            Console.WriteLine("ELIMINAR ESTUDIANTE");
            Console.Write("Ingrese el Carnet o Codigo a dar de baja: ");
            string q = Console.ReadLine()?.Trim() ?? "";
            int i = BuscarIdx(q);

            if (i != -1)
            {
                listaEstudiantes[i].Activo = false;
                Console.WriteLine("[EXITO] El estudiante " + listaEstudiantes[i].Nombre + " ha sido dado de baja del sistema.");
            }
            else
            {
                Console.WriteLine("\nEstudiante no encontrado.");
            }
        }

        static void EstadisticasFacultad()
        {
            Console.WriteLine("=========================================================");
            Console.WriteLine("         ESTADISTICAS GLOBALES POR FACULTAD              ");
            Console.WriteLine("=========================================================");

            foreach (string f in LISTA_FACULTADES)
            {
                Console.WriteLine(">> FACULTAD: " + f.ToUpper());
                int tot = 0;
                double sumCUM = 0;
                double maxCUM = -1;
                double minCUM = 11;

                int apNotas = 0, repNotas = 0;
                int apAsis = 0, repAsis = 0;

                int cantM = 0, cantF = 0;
                double sumM = 0, sumF = 0;

                for (int i = 0; i < CAPACIDAD; i++)
                {
                    if (listaEstudiantes[i].Activo && listaEstudiantes[i].Facultad == f)
                    {
                        tot++;
                        double cum = CalcularCUM(listaEstudiantes[i]);
                        sumCUM += cum;

                        if (cum > maxCUM) maxCUM = cum;
                        if (cum < minCUM) minCUM = cum;

                        if (cum >= NOTA_MINIMA_APROBACION) apNotas++; else repNotas++;
                        if (listaEstudiantes[i].Asistencia >= ASISTENCIA_MINIMA_APROBACION) apAsis++; else repAsis++;

                        if (listaEstudiantes[i].Genero == "Masculino") { cantM++; sumM += cum; }
                        else { cantF++; sumF += cum; }
                    }
                }

                if (tot == 0)
                {
                    Console.WriteLine("   Sin estudiantes registrados.");
                    continue;
                }

                double cumPromedioGeneral = sumCUM / tot;
                double pctApNota = ((double)apNotas / tot) * 100.0;
                double pctRepNota = ((double)repNotas / tot) * 100.0;
                double pctApAsis = ((double)apAsis / tot) * 100.0;
                double pctRepAsis = ((double)repAsis / tot) * 100.0;

                double pctM = ((double)cantM / tot) * 100.0;
                double pctF = ((double)cantF / tot) * 100.0;

                Console.WriteLine("   - Total Inscritos: " + tot);
                Console.WriteLine("   - CUM Promedio General: " + cumPromedioGeneral.ToString("F2"));
                Console.WriteLine("   - CUM Mas Alto: " + maxCUM.ToString("F2") + " | CUM Mas Bajo: " + minCUM.ToString("F2"));
                Console.WriteLine("   - Aprobados por Nota: " + apNotas + " (" + pctApNota.ToString("F2") + "%) | Reprobados: " + repNotas + " (" + pctRepNota.ToString("F2") + "%)");
                Console.WriteLine("   - Aprobados por Asistencia: " + apAsis + " (" + pctApAsis.ToString("F2") + "%) | Reprobados: " + repAsis + " (" + pctRepAsis.ToString("F2") + "%)");
                Console.WriteLine("   - Varones: " + cantM + " (" + pctM.ToString("F2") + "%) - Promedio CUM: " + (cantM > 0 ? (sumM / cantM).ToString("F2") : "N/A"));
                Console.WriteLine("   - Hembras: " + cantF + " (" + pctF.ToString("F2") + "%) - Promedio CUM: " + (cantF > 0 ? (sumF / cantF).ToString("F2") : "N/A"));

                MostrarRankingFacultad(f);
            }
        }

        static void EstadisticasCarrera()
        {
            Console.WriteLine("=========================================================");
            Console.WriteLine("          ESTADISTICAS GLOBALES POR CARRERA              ");
            Console.WriteLine("=========================================================");

            foreach (string c in LISTA_CARRERAS)
            {
                Console.WriteLine(">> CARRERA: " + c.ToUpper());
                int tot = 0;
                double sumCUM = 0;
                double maxCUM = -1, minCUM = 11;
                int apNotas = 0, repNotas = 0;
                int apAsis = 0, repAsis = 0;

                int cantM = 0, cantF = 0;
                double sumM = 0, sumF = 0;

                for (int i = 0; i < CAPACIDAD; i++)
                {
                    if (listaEstudiantes[i].Activo && listaEstudiantes[i].Carrera == c)
                    {
                        tot++;
                        double cum = CalcularCUM(listaEstudiantes[i]);
                        sumCUM += cum;

                        if (cum > maxCUM) maxCUM = cum;
                        if (cum < minCUM) minCUM = cum;

                        if (cum >= NOTA_MINIMA_APROBACION) apNotas++; else repNotas++;
                        if (listaEstudiantes[i].Asistencia >= ASISTENCIA_MINIMA_APROBACION) apAsis++; else repAsis++;

                        if (listaEstudiantes[i].Genero == "Masculino") { cantM++; sumM += cum; }
                        else { cantF++; sumF += cum; }
                    }
                }

                if (tot == 0)
                {
                    Console.WriteLine("   Sin estudiantes registrados.");
                    continue;
                }

                double cumPromedioGeneral = sumCUM / tot;
                double pctApNota = ((double)apNotas / tot) * 100.0;
                double pctRepNota = ((double)repNotas / tot) * 100.0;
                double pctApAsis = ((double)apAsis / tot) * 100.0;
                double pctRepAsis = ((double)repAsis / tot) * 100.0;

                double pctM = ((double)cantM / tot) * 100.0;
                double pctF = ((double)cantF / tot) * 100.0;

                Console.WriteLine("   - Total Alumnos: " + tot);
                Console.WriteLine("   - CUM Promedio General: " + cumPromedioGeneral.ToString("F2"));
                Console.WriteLine("   - CUM Mas Alto: " + maxCUM.ToString("F2") + " | CUM Mas Bajo: " + minCUM.ToString("F2"));
                Console.WriteLine("   - Aprobados por Nota: " + apNotas + " (" + pctApNota.ToString("F2") + "%) | Reprobados: " + repNotas + " (" + pctRepNota.ToString("F2") + "%)");
                Console.WriteLine("   - Aprobados por Asistencia: " + apAsis + " (" + pctApAsis.ToString("F2") + "%) | Reprobados: " + repAsis + " (" + pctRepAsis.ToString("F2") + "%)");
                Console.WriteLine("   - Varones: " + cantM + " (" + pctM.ToString("F2") + "%) - Promedio CUM: " + (cantM > 0 ? (sumM / cantM).ToString("F2") : "N/A"));
                Console.WriteLine("   - Hembras: " + cantF + " (" + pctF.ToString("F2") + "%) - Promedio CUM: " + (cantF > 0 ? (sumF / cantF).ToString("F2") : "N/A"));

                MostrarRankingCarrera(c);
            }
        }

        static void ReporteCarrera()
        {
            Console.WriteLine(" REPORTE ACADEMICO DETALLADO POR CARRERA ");
            for (int i = 0; i < LISTA_CARRERAS.Length; i++)
            {
                Console.WriteLine((i + 1) + ". " + LISTA_CARRERAS[i]);
            }
            Console.Write("Seleccione la carrera a consultar: ");
            int s;
            if (!int.TryParse(Console.ReadLine(), out s) || s < 1 || s > LISTA_CARRERAS.Length)
            {
                Console.WriteLine("Seleccion invalida.");
                return;
            }

            string cSel = LISTA_CARRERAS[s - 1];
            int tot = 0;
            double sumCUM = 0;
            int idxMejor = -1, idxPeor = -1;
            double mejorCUM = -1, peorCUM = 11;
            int aprobadosGlobales = 0;

            for (int i = 0; i < CAPACIDAD; i++)
            {
                if (listaEstudiantes[i].Activo && listaEstudiantes[i].Carrera == cSel)
                {
                    tot++;
                    double cum = CalcularCUM(listaEstudiantes[i]);
                    sumCUM += cum;

                    if (cum > mejorCUM) { mejorCUM = cum; idxMejor = i; }
                    if (cum < peorCUM) { peorCUM = cum; idxPeor = i; }

                    if (cum >= NOTA_MINIMA_APROBACION && listaEstudiantes[i].Asistencia >= ASISTENCIA_MINIMA_APROBACION)
                    {
                        aprobadosGlobales++;
                    }
                }
            }

            Console.WriteLine("=========================================================");
            Console.WriteLine(" REPORTE ACADEMICO: " + cSel.ToUpper());
            Console.WriteLine("=========================================================");
            Console.WriteLine(" Total de Alumnos Inscritos: " + tot);

            if (tot == 0)
            {
                Console.WriteLine(" No hay informacion disponible para esta carrera.");
                return;
            }

            double cumGeneralCarrera = sumCUM / tot;
            double pctAprobacionGlobal = ((double)aprobadosGlobales / tot) * 100.0;

            Console.WriteLine(" CUM General de la Carrera: " + cumGeneralCarrera.ToString("F2"));
            Console.WriteLine(" Porcentaje de Aprobacion Global: " + pctAprobacionGlobal.ToString("F2") + "%");

            Console.WriteLine(" MEJOR ESTUDIANTE (RENDIMIENTO MAS ALTO) ");
            Console.WriteLine(" Nombre: " + listaEstudiantes[idxMejor].Nombre + " | Carnet: " + listaEstudiantes[idxMejor].Carnet + " | CUM: " + mejorCUM.ToString("F2"));

            Console.WriteLine(" PEOR ESTUDIANTE (RENDIMIENTO MAS BAJO) ");
            Console.WriteLine(" Nombre: " + listaEstudiantes[idxPeor].Nombre + " | Carnet: " + listaEstudiantes[idxPeor].Carnet + " | CUM: " + peorCUM.ToString("F2"));
        }

        static double CalcularCUM(Estudiante e)
        {
            if (e.Notas == null || e.Notas.Length == 0) return 0.0;
            double suma = 0;
            for (int i = 0; i < e.Notas.Length; i++)
            {
                suma += e.Notas[i];
            }
            return suma / e.Notas.Length;
        }

        static bool ExisteCod(string c)
        {
            for (int i = 0; i < CAPACIDAD; i++)
            {
                if (listaEstudiantes[i].Activo && listaEstudiantes[i].Codigo.Equals(c, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        static bool ExisteCar(string c)
        {
            for (int i = 0; i < CAPACIDAD; i++)
            {
                if (listaEstudiantes[i].Activo && listaEstudiantes[i].Carnet.Equals(c, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        static int BuscarIdx(string q)
        {
            for (int i = 0; i < CAPACIDAD; i++)
            {
                if (listaEstudiantes[i].Activo && (listaEstudiantes[i].Carnet.Equals(q, StringComparison.OrdinalIgnoreCase) ||
                                                   listaEstudiantes[i].Codigo.Equals(q, StringComparison.OrdinalIgnoreCase)))
                {
                    return i;
                }
            }
            return -1;
        }

        static double LeerNota(string msg)
        {
            double v;
            do
            {
                Console.Write(msg);
            } while (!double.TryParse(Console.ReadLine(), out v) || v < 0 || v > 10);
            return v;
        }

        static void MostrarRankingFacultad(string f)
        {
            int count = 0;
            for (int i = 0; i < CAPACIDAD; i++)
            {
                if (listaEstudiantes[i].Activo && listaEstudiantes[i].Facultad == f) count++;
            }

            int[] arr = new int[count];
            int pos = 0;
            for (int i = 0; i < CAPACIDAD; i++)
            {
                if (listaEstudiantes[i].Activo && listaEstudiantes[i].Facultad == f) arr[pos++] = i;
            }

            OrdenarPorCUMBurbuja(arr);

            Console.WriteLine("TOP / RANKING MEJORES ESTUDIANTES (POR CUM)");
            int tope = Math.Min(arr.Length, 3);
            if (tope == 0) Console.WriteLine("   (Sin estudiantes)");
            for (int k = 0; k < tope; k++)
            {
                int idx = arr[k];
                double cum = CalcularCUM(listaEstudiantes[idx]);
                Console.WriteLine("   #" + (k + 1) + " - " + listaEstudiantes[idx].Nombre + " (Carnet: " + listaEstudiantes[idx].Carnet + ") - CUM: " + cum.ToString("F2"));
            }
        }

        static void MostrarRankingCarrera(string c)
        {
            int count = 0;
            for (int i = 0; i < CAPACIDAD; i++)
            {
                if (listaEstudiantes[i].Activo && listaEstudiantes[i].Carrera == c) count++;
            }

            int[] arr = new int[count];
            int pos = 0;
            for (int i = 0; i < CAPACIDAD; i++)
            {
                if (listaEstudiantes[i].Activo && listaEstudiantes[i].Carrera == c) arr[pos++] = i;
            }

            OrdenarPorCUMBurbuja(arr);

            Console.WriteLine("TOP / RANKING MEJORES ESTUDIANTES (POR CUM)");
            int tope = Math.Min(arr.Length, 3);
            if (tope == 0) Console.WriteLine("   (Sin estudiantes)");
            for (int k = 0; k < tope; k++)
            {
                int idx = arr[k];
                double cum = CalcularCUM(listaEstudiantes[idx]);
                Console.WriteLine("   #" + (k + 1) + " - " + listaEstudiantes[idx].Nombre + " (Carnet: " + listaEstudiantes[idx].Carnet + ") - CUM: " + cum.ToString("F2"));
            }
        }

        static void OrdenarPorCUMBurbuja(int[] arr)
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = 0; j < arr.Length - i - 1; j++)
                {
                    double cumA = CalcularCUM(listaEstudiantes[arr[j]]);
                    double cumB = CalcularCUM(listaEstudiantes[arr[j + 1]]);

                    if (cumA < cumB)
                    {
                        int temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }
            }
        }
    }
}
