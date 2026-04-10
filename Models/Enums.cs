namespace HospitalEHR.Models
{
    public enum AdmissionStatus   { Registered, Admitted, UnderTreatment, Discharged }
    public enum TestStatus        { Ordered, InProgress, Completed, Cancelled }
    public enum PaymentStatus     { Pending, PartiallyPaid, Paid, Refunded }
    public enum PrescriptionStatus{ Prescribed, Dispensed, Cancelled }
    public enum Gender            { Male, Female, Other, PreferNotToSay }
	public enum BloodGroup        { APositive, ANegative, BPositive, BNegative, ABPositive, ABNegative, OPositive, ONegative, Unknown }
	
	public enum UserRole          { Admin, Receptionist, Doctor, LabTechnician, Pharmacist, BillingOfficer }
}
