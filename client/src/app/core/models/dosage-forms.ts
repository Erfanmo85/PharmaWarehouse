export const DOSAGE_FORMS = [
  { value: 'Tablet', label: 'قرص' },
  { value: 'Capsule', label: 'کپسول' },
  { value: 'Syrup', label: 'شربت' },
  { value: 'Injection', label: 'آمپول' },
  { value: 'Ointment', label: 'پماد' },
];

export const dosageLabel = (value: string) =>
  DOSAGE_FORMS.find(f => f.value === value)?.label ?? value;
