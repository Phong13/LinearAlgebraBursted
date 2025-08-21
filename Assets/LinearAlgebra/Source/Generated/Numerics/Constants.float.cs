// <copyright file= (float) "Constants.cs" company= (float) "Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2010 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace LinearAlgebra.MathNet.Numerics
{
    /// <summary>
    /// A collection of frequently used mathematical constants.
    /// </summary>
    public static unsafe class Constantsfloat
    {
        #region Mathematical Constants

        /// <summary>The number e</summary>
        public static readonly float E = (float)  2.7182818284590452353602874713526624977572470937000d;

        /// <summary>The number log[2](e)</summary>
        public static readonly float Log2E = (float)  1.4426950408889634073599246810018921374266459541530d;

        /// <summary>The number log[10](e)</summary>
        public static readonly float Log10E = (float)  0.43429448190325182765112891891660508229439700580366d;

        /// <summary>The number log[e](2)</summary>
        public static readonly float Ln2 = (float)  0.69314718055994530941723212145817656807550013436026d;

        /// <summary>The number log[e](10)</summary>
        public static readonly float Ln10 = (float)  2.3025850929940456840179914546843642076011014886288d;

        /// <summary>The number log[e](pi)</summary>
        public static readonly float LnPi = (float)  1.1447298858494001741434273513530587116472948129153d;

        /// <summary>The number log[e](2*pi)/2</summary>
        public static readonly float Ln2PiOver2 = (float)  0.91893853320467274178032973640561763986139747363780d;

        /// <summary>The number 1/e</summary>
        public static readonly float InvE = (float)  0.36787944117144232159552377016146086744581113103176d;

        /// <summary>The number sqrt(e)</summary>
        public static readonly float SqrtE = (float)  1.6487212707001281468486507878141635716537761007101d;

        /// <summary>The number sqrt(2)</summary>
        public static readonly float Sqrt2 = (float)  1.4142135623730950488016887242096980785696718753769d;

        /// <summary>The number sqrt(3)</summary>
        public static readonly float Sqrt3 = (float)  1.7320508075688772935274463415058723669428052538104d;

        /// <summary>The number sqrt(1/2) = (float)  1/sqrt(2) = (float)  sqrt(2)/2</summary>
        public static readonly float Sqrt1Over2 = (float)  0.70710678118654752440084436210484903928483593768845d;

        /// <summary>The number sqrt(3)/2</summary>
        public static readonly float HalfSqrt3 = (float)  0.86602540378443864676372317075293618347140262690520d;

        /// <summary>The number pi</summary>
        public static readonly float Pi = (float)  3.1415926535897932384626433832795028841971693993751d;

        /// <summary>The number pi*2</summary>
        public static readonly float Pi2 = (float)  6.2831853071795864769252867665590057683943387987502d;

        /// <summary>The number pi/2</summary>
        public static readonly float PiOver2 = (float)  1.5707963267948966192313216916397514420985846996876d;

        /// <summary>The number pi*3/2</summary>
        public static readonly float Pi3Over2 = (float)  4.71238898038468985769396507491925432629575409906266d;

        /// <summary>The number pi/4</summary>
        public static readonly float PiOver4 = (float)  0.78539816339744830961566084581987572104929234984378d;

        /// <summary>The number sqrt(pi)</summary>
        public static readonly float SqrtPi = (float)  1.7724538509055160272981674833411451827975494561224d;

        /// <summary>The number sqrt(2pi)</summary>
        public static readonly float Sqrt2Pi = (float)  2.5066282746310005024157652848110452530069867406099d;

        /// <summary>The number sqrt(pi/2)</summary>
        public static readonly float SqrtPiOver2 = (float)  1.2533141373155002512078826424055226265034933703050d;

        /// <summary>The number sqrt(2*pi*e)</summary>
        public static readonly float Sqrt2PiE = (float)  4.1327313541224929384693918842998526494455219169913d;

        /// <summary>The number log(sqrt(2*pi))</summary>
        public static readonly float LogSqrt2Pi = (float)  0.91893853320467274178032973640561763986139747363778;

        /// <summary>The number log(sqrt(2*pi*e))</summary>
        public static readonly float LogSqrt2PiE = (float)  1.4189385332046727417803297364056176398613974736378d;

        /// <summary>The number log(2 * sqrt(e / pi))</summary>
        public static readonly float LogTwoSqrtEOverPi = (float)  0.6207822376352452223455184457816472122518527279025978;

        /// <summary>The number 1/pi</summary>
        public static readonly float InvPi = (float)  0.31830988618379067153776752674502872406891929148091d;

        /// <summary>The number 2/pi</summary>
        public static readonly float TwoInvPi = (float)  0.63661977236758134307553505349005744813783858296182d;

        /// <summary>The number 1/sqrt(pi)</summary>
        public static readonly float InvSqrtPi = (float)  0.56418958354775628694807945156077258584405062932899d;

        /// <summary>The number 1/sqrt(2pi)</summary>
        public static readonly float InvSqrt2Pi = (float)  0.39894228040143267793994605993438186847585863116492d;

        /// <summary>The number 2/sqrt(pi)</summary>
        public static readonly float TwoInvSqrtPi = (float)  1.1283791670955125738961589031215451716881012586580d;

        /// <summary>The number 2 * sqrt(e / pi)</summary>
        public static readonly float TwoSqrtEOverPi = (float)  1.8603827342052657173362492472666631120594218414085755;

        /// <summary>The number (pi)/180 - factor to convert from Degree (deg) to Radians (rad).</summary>
        /// <seealso cref= (float) "Trig.DegreeToRadian"/>
        /// <seealso cref= (float) "Trig.RadianToDegree"/>
        public static readonly float Degree = (float)  0.017453292519943295769236907684886127134428718885417d;

        /// <summary>The number (pi)/200 - factor to convert from NewGrad (grad) to Radians (rad).</summary>
        /// <seealso cref= (float) "Trig.GradToRadian"/>
        /// <seealso cref= (float) "Trig.RadianToGrad"/>
        public static readonly float Grad = (float)  0.015707963267948966192313216916397514420985846996876d;

        /// <summary>The number ln(10)/20 - factor to convert from Power Decibel (dB) to Neper (Np). Use this version when the Decibel represent a power gain but the compared values are not powers (e.g. amplitude, current, voltage).</summary>
        public static readonly float PowerDecibel = (float)  0.11512925464970228420089957273421821038005507443144d;

        /// <summary>The number ln(10)/10 - factor to convert from Neutral Decibel (dB) to Neper (Np). Use this version when either both or neither of the Decibel and the compared values represent powers.</summary>
        public static readonly float NeutralDecibel = (float)  0.23025850929940456840179914546843642076011014886288d;

        /// <summary>The Catalan constant</summary>
        /// <remarks>Sum(k= (float) 0 -> inf){ (-1)^k/(2*k + 1)2 }</remarks>
        public static readonly float Catalan = (float)  0.9159655941772190150546035149323841107741493742816721342664981196217630197762547694794d;

        /// <summary>The Euler-Mascheroni constant</summary>
        /// <remarks>lim(n -> inf){ Sum(k= (float) 1 -> n) { 1/k - log(n) } }</remarks>
        public static readonly float EulerMascheroni = (float)  0.5772156649015328606065120900824024310421593359399235988057672348849d;

        /// <summary>The number (1+sqrt(5))/2, also known as the golden ratio</summary>
        public static readonly float GoldenRatio = (float)  1.6180339887498948482045868343656381177203091798057628621354486227052604628189024497072d;

        /// <summary>The Glaisher constant</summary>
        /// <remarks>e^(1/12 - Zeta(-1))</remarks>
        public static readonly float Glaisher = (float)  1.2824271291006226368753425688697917277676889273250011920637400217404063088588264611297d;

        /// <summary>The Khinchin constant</summary>
        /// <remarks>prod(k= (float) 1 -> inf){1+1/(k*(k+2))^log(k,2)}</remarks>
        public static readonly float Khinchin = (float)  2.6854520010653064453097148354817956938203822939944629530511523455572188595371520028011d;

        /// <summary>
        /// The size of a float in bytes.
        /// </summary>
        public static readonly int SizeOfDouble = sizeof(float);

        /// <summary>
        /// The size of an int in bytes.
        /// </summary>
        public static readonly int SizeOfInt = sizeof(int);

        /// <summary>
        /// The size of a float in bytes.
        /// </summary>
        public static readonly int SizeOfFloat = sizeof(float);

        /// <summary>
        /// The size of a Complex in bytes.
        /// </summary>
        public static readonly int SizeOfComplex =  2 * SizeOfDouble;

        /// <summary>
        /// The size of a Complex in bytes.
        /// </summary>
        public static readonly int SizeOfComplex32 =  2 * SizeOfFloat;
        #endregion

        #region UNIVERSAL CONSTANTS

        /// <summary>Speed of Light in Vacuum: c_0 = (float)  2.99792458e8 [m s^-1] (defined, exact; 2007 CODATA)</summary>
        public static readonly float SpeedOfLight = (float)  2.99792458e8;

        /// <summary>Magnetic Permeability in Vacuum: mu_0 = (float)  4*Pi * 10^-7 [N A^-2 = (float)  kg m A^-2 s^-2] (defined, exact; 2007 CODATA)</summary>
        public static readonly float MagneticPermeability = (float)  1.2566370614359172953850573533118011536788677597500e-6;

        /// <summary>Electric Permittivity in Vacuum: epsilon_0 = (float)  1/(mu_0*c_0^2) [F m^-1 = (float)  A^2 s^4 kg^-1 m^-3] (defined, exact; 2007 CODATA)</summary>
        public static readonly float ElectricPermittivity = (float)  8.8541878171937079244693661186959426889222899381429e-12;

        /// <summary>Characteristic Impedance of Vacuum: Z_0 = (float)  mu_0*c_0 [Ohm = (float)  m^2 kg s^-3 A^-2] (defined, exact; 2007 CODATA)</summary>
        public static readonly float CharacteristicImpedanceVacuum = (float)  376.73031346177065546819840042031930826862350835242;

        /// <summary>Newtonian Constant of Gravitation: G = (float)  6.67429e-11 [m^3 kg^-1 s^-2] (2007 CODATA)</summary>
        public static readonly float GravitationalConstant = (float)  6.67429e-11;

        /// <summary>Planck's constant: h = (float)  6.62606896e-34 [J s = (float)  m^2 kg s^-1] (2007 CODATA)</summary>
        public static readonly float PlancksConstant = (float)  6.62606896e-34;

        /// <summary>Reduced Planck's constant: h_bar = (float)  h / (2*Pi) [J s = (float)  m^2 kg s^-1] (2007 CODATA)</summary>
        public static readonly float DiracsConstant = (float)  1.054571629e-34;

        /// <summary>Planck mass: m_p = (float)  (h_bar*c_0/G)^(1/2) [kg] (2007 CODATA)</summary>
        public static readonly float PlancksMass = (float)  2.17644e-8;

        /// <summary>Planck temperature: T_p = (float)  (h_bar*c_0^5/G)^(1/2)/k [K] (2007 CODATA)</summary>
        public static readonly float PlancksTemperature = (float)  1.416786e32;

        /// <summary>Planck length: l_p = (float)  h_bar/(m_p*c_0) [m] (2007 CODATA)</summary>
        public static readonly float PlancksLength = (float)  1.616253e-35;

        /// <summary>Planck time: t_p = (float)  l_p/c_0 [s] (2007 CODATA)</summary>
        public static readonly float PlancksTime = (float)  5.39124e-44;

        #endregion

        #region ELECTROMAGNETIC CONSTANTS

        /// <summary>Elementary Electron Charge: e = (float)  1.602176487e-19 [C = (float)  A s] (2007 CODATA)</summary>
        public static readonly float ElementaryCharge = (float)  1.602176487e-19;

        /// <summary>Magnetic Flux Quantum: theta_0 = (float)  h/(2*e) [Wb = (float)  m^2 kg s^-2 A^-1] (2007 CODATA)</summary>
        public static readonly float MagneticFluxQuantum = (float)  2.067833668e-15;

        /// <summary>Conductance Quantum: G_0 = (float)  2*e^2/h [S = (float)  m^-2 kg^-1 s^3 A^2] (2007 CODATA)</summary>
        public static readonly float ConductanceQuantum = (float)  7.7480917005e-5;

        /// <summary>Josephson Constant: K_J = (float)  2*e/h [Hz V^-1] (2007 CODATA)</summary>
        public static readonly float JosephsonConstant = (float)  483597.891e9;

        /// <summary>Von Klitzing Constant: R_K = (float)  h/e^2 [Ohm = (float)  m^2 kg s^-3 A^-2] (2007 CODATA)</summary>
        public static readonly float VonKlitzingConstant = (float)  25812.807557;

        /// <summary>Bohr Magneton: mu_B = (float)  e*h_bar/2*m_e [J T^-1] (2007 CODATA)</summary>
        public static readonly float BohrMagneton = (float)  927.400915e-26;

        /// <summary>Nuclear Magneton: mu_N = (float)  e*h_bar/2*m_p [J T^-1] (2007 CODATA)</summary>
        public static readonly float NuclearMagneton = (float)  5.05078324e-27;

        #endregion

        #region ATOMIC AND NUCLEAR CONSTANTS

        /// <summary>Fine Structure Constant: alpha = (float)  e^2/4*Pi*e_0*h_bar*c_0 [1] (2007 CODATA)</summary>
        public static readonly float FineStructureConstant = (float)  7.2973525376e-3;

        /// <summary>Rydberg Constant: R_infty = (float)  alpha^2*m_e*c_0/2*h [m^-1] (2007 CODATA)</summary>
        public static readonly float RydbergConstant = (float)  10973731.568528;

        /// <summary>Bor Radius: a_0 = (float)  alpha/4*Pi*R_infty [m] (2007 CODATA)</summary>
        public static readonly float BohrRadius = (float)  0.52917720859e-10;

        /// <summary>Hartree Energy: E_h = (float)  2*R_infty*h*c_0 [J] (2007 CODATA)</summary>
        public static readonly float HartreeEnergy = (float)  4.35974394e-18;

        /// <summary>Quantum of Circulation: h/2*m_e [m^2 s^-1] (2007 CODATA)</summary>
        public static readonly float QuantumOfCirculation = (float)  3.6369475199e-4;

        /// <summary>Fermi Coupling Constant: G_F/(h_bar*c_0)^3 [GeV^-2] (2007 CODATA)</summary>
        public static readonly float FermiCouplingConstant = (float)  1.16637e-5;

        /// <summary>Weak Mixin Angle: sin^2(theta_W) [1] (2007 CODATA)</summary>
        public static readonly float WeakMixingAngle = (float)  0.22256;

        /// <summary>Electron Mass: [kg] (2007 CODATA)</summary>
        public static readonly float ElectronMass = (float)  9.10938215e-31;

        /// <summary>Electron Mass Energy Equivalent: [J] (2007 CODATA)</summary>
        public static readonly float ElectronMassEnergyEquivalent = (float)  8.18710438e-14;

        /// <summary>Electron Molar Mass: [kg mol^-1] (2007 CODATA)</summary>
        public static readonly float ElectronMolarMass = (float)  5.4857990943e-7;

        /// <summary>Electron Compton Wavelength: [m] (2007 CODATA)</summary>
        public static readonly float ComptonWavelength = (float)  2.4263102175e-12;

        /// <summary>Classical Electron Radius: [m] (2007 CODATA)</summary>
        public static readonly float ClassicalElectronRadius = (float)  2.8179402894e-15;

        /// <summary>Thomson Cross Section: [m^2] (2002 CODATA)</summary>
        public static readonly float ThomsonCrossSection = (float)  0.6652458558e-28;

        /// <summary>Electron Magnetic Moment: [J T^-1] (2007 CODATA)</summary>
        public static readonly float ElectronMagneticMoment = (float)  (-928.476377e-26);

        /// <summary>Electon G-Factor: [1] (2007 CODATA)</summary>
        public static readonly float ElectronGFactor = (float)  (-2.0023193043622);

        /// <summary>Muon Mass: [kg] (2007 CODATA)</summary>
        public static readonly float MuonMass = (float)  1.88353130e-28;

        /// <summary>Muon Mass Energy Equivalent: [J] (2007 CODATA)</summary>
        public static readonly float MuonMassEnegryEquivalent = (float)  1.692833511e-11;

        /// <summary>Muon Molar Mass: [kg mol^-1] (2007 CODATA)</summary>
        public static readonly float MuonMolarMass = (float)  0.1134289256e-3;

        /// <summary>Muon Compton Wavelength: [m] (2007 CODATA)</summary>
        public static readonly float MuonComptonWavelength = (float)  11.73444104e-15;

        /// <summary>Muon Magnetic Moment: [J T^-1] (2007 CODATA)</summary>
        public static readonly float MuonMagneticMoment = (float)  (-4.49044786e-26);

        /// <summary>Muon G-Factor: [1] (2007 CODATA)</summary>
        public static readonly float MuonGFactor = (float)  (-2.0023318414);

        /// <summary>Tau Mass: [kg] (2007 CODATA)</summary>
        public static readonly float TauMass = (float)  3.16777e-27;

        /// <summary>Tau Mass Energy Equivalent: [J] (2007 CODATA)</summary>
        public static readonly float TauMassEnergyEquivalent = (float)  2.84705e-10;

        /// <summary>Tau Molar Mass: [kg mol^-1] (2007 CODATA)</summary>
        public static readonly float TauMolarMass = (float)  1.90768e-3;

        /// <summary>Tau Compton Wavelength: [m] (2007 CODATA)</summary>
        public static readonly float TauComptonWavelength = (float)  0.69772e-15;

        /// <summary>Proton Mass: [kg] (2007 CODATA)</summary>
        public static readonly float ProtonMass = (float)  1.672621637e-27;

        /// <summary>Proton Mass Energy Equivalent: [J] (2007 CODATA)</summary>
        public static readonly float ProtonMassEnergyEquivalent = (float)  1.503277359e-10;

        /// <summary>Proton Molar Mass: [kg mol^-1] (2007 CODATA)</summary>
        public static readonly float ProtonMolarMass = (float)  1.00727646677e-3;

        /// <summary>Proton Compton Wavelength: [m] (2007 CODATA)</summary>
        public static readonly float ProtonComptonWavelength = (float)  1.3214098446e-15;

        /// <summary>Proton Magnetic Moment: [J T^-1] (2007 CODATA)</summary>
        public static readonly float ProtonMagneticMoment = (float)  1.410606662e-26;

        /// <summary>Proton G-Factor: [1] (2007 CODATA)</summary>
        public static readonly float ProtonGFactor = (float)  5.585694713;

        /// <summary>Proton Shielded Magnetic Moment: [J T^-1] (2007 CODATA)</summary>
        public static readonly float ShieldedProtonMagneticMoment = (float)  1.410570419e-26;

        /// <summary>Proton Gyro-Magnetic Ratio: [s^-1 T^-1] (2007 CODATA)</summary>
        public static readonly float ProtonGyromagneticRatio = (float)  2.675222099e8;

        /// <summary>Proton Shielded Gyro-Magnetic Ratio: [s^-1 T^-1] (2007 CODATA)</summary>
        public static readonly float ShieldedProtonGyromagneticRatio = (float)  2.675153362e8;

        /// <summary>Neutron Mass: [kg] (2007 CODATA)</summary>
        public static readonly float NeutronMass = (float)  1.674927212e-27;

        /// <summary>Neutron Mass Energy Equivalent: [J] (2007 CODATA)</summary>
        public static readonly float NeutronMassEnegryEquivalent = (float)  1.505349506e-10;

        /// <summary>Neutron Molar Mass: [kg mol^-1] (2007 CODATA)</summary>
        public static readonly float NeutronMolarMass = (float)  1.00866491597e-3;

        /// <summary>Neuron Compton Wavelength: [m] (2007 CODATA)</summary>
        public static readonly float NeutronComptonWavelength = (float)  1.3195908951e-1;

        /// <summary>Neutron Magnetic Moment: [J T^-1] (2007 CODATA)</summary>
        public static readonly float NeutronMagneticMoment = (float)  (-0.96623641e-26);

        /// <summary>Neutron G-Factor: [1] (2007 CODATA)</summary>
        public static readonly float NeutronGFactor = (float)  (-3.82608545);

        /// <summary>Neutron Gyro-Magnetic Ratio: [s^-1 T^-1] (2007 CODATA)</summary>
        public static readonly float NeutronGyromagneticRatio = (float)  1.83247185e8;

        /// <summary>Deuteron Mass: [kg] (2007 CODATA)</summary>
        public static readonly float DeuteronMass = (float)  3.34358320e-27;

        /// <summary>Deuteron Mass Energy Equivalent: [J] (2007 CODATA)</summary>
        public static readonly float DeuteronMassEnegryEquivalent = (float)  3.00506272e-10;

        /// <summary>Deuteron Molar Mass: [kg mol^-1] (2007 CODATA)</summary>
        public static readonly float DeuteronMolarMass = (float)  2.013553212725e-3;

        /// <summary>Deuteron Magnetic Moment: [J T^-1] (2007 CODATA)</summary>
        public static readonly float DeuteronMagneticMoment = (float)  0.433073465e-26;

        /// <summary>Helion Mass: [kg] (2007 CODATA)</summary>
        public static readonly float HelionMass = (float)  5.00641192e-27;

        /// <summary>Helion Mass Energy Equivalent: [J] (2007 CODATA)</summary>
        public static readonly float HelionMassEnegryEquivalent = (float)  4.49953864e-10;

        /// <summary>Helion Molar Mass: [kg mol^-1] (2007 CODATA)</summary>
        public static readonly float HelionMolarMass = (float)  3.0149322473e-3;

        /// <summary>Avogadro constant: [mol^-1] (2010 CODATA)</summary>
        public static readonly float Avogadro = (float)  6.0221412927e23;

        #endregion

        #region Scientific Prefixes
        /// <summary>The SI prefix factor corresponding to 1 000 000 000 000 000 000 000 000</summary>
        public static readonly float Yotta = (float)  1e24;

        /// <summary>The SI prefix factor corresponding to 1 000 000 000 000 000 000 000</summary>
        public static readonly float Zetta = (float)  1e21;

        /// <summary>The SI prefix factor corresponding to 1 000 000 000 000 000 000</summary>
        public static readonly float Exa = (float)  1e18;

        /// <summary>The SI prefix factor corresponding to 1 000 000 000 000 000</summary>
        public static readonly float Peta = (float)  1e15;

        /// <summary>The SI prefix factor corresponding to 1 000 000 000 000</summary>
        public static readonly float Tera = (float)  1e12;

        /// <summary>The SI prefix factor corresponding to 1 000 000 000</summary>
        public static readonly float Giga = (float)  1e9;

        /// <summary>The SI prefix factor corresponding to 1 000 000</summary>
        public static readonly float Mega = (float)  1e6;

        /// <summary>The SI prefix factor corresponding to 1 000</summary>
        public static readonly float Kilo = (float)  1e3;

        /// <summary>The SI prefix factor corresponding to 100</summary>
        public static readonly float Hecto = (float)  1e2;

        /// <summary>The SI prefix factor corresponding to 10</summary>
        public static readonly float Deca = (float)  1e1;

        /// <summary>The SI prefix factor corresponding to 0.1</summary>
        public static readonly float Deci = (float)  1e-1;

        /// <summary>The SI prefix factor corresponding to 0.01</summary>
        public static readonly float Centi = (float)  1e-2;

        /// <summary>The SI prefix factor corresponding to 0.001</summary>
        public static readonly float Milli = (float)  1e-3;

        /// <summary>The SI prefix factor corresponding to 0.000 001</summary>
        public static readonly float Micro = (float)  1e-6;

        /// <summary>The SI prefix factor corresponding to 0.000 000 001</summary>
        public static readonly float Nano = (float)  1e-9;

        /// <summary>The SI prefix factor corresponding to 0.000 000 000 001</summary>
        public static readonly float Pico = (float)  1e-12;

        /// <summary>The SI prefix factor corresponding to 0.000 000 000 000 001</summary>
        public static readonly float Femto = (float)  1e-15;

        /// <summary>The SI prefix factor corresponding to 0.000 000 000 000 000 001</summary>
        public static readonly float Atto = (float)  1e-18;

        /// <summary>The SI prefix factor corresponding to 0.000 000 000 000 000 000 001</summary>
        public static readonly float Zepto = (float)  1e-21;

        /// <summary>The SI prefix factor corresponding to 0.000 000 000 000 000 000 000 001</summary>
        public static readonly float Yocto = (float)  1e-24;
        #endregion
    }
}
