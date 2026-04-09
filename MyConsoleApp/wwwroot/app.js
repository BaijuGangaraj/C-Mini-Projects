const medicineTableBody = document.getElementById('medicineTableBody');
const searchInput = document.getElementById('searchInput');
const medicineForm = document.getElementById('medicineForm');
const medicineMessage = document.getElementById('medicineMessage');
const saleForm = document.getElementById('saleForm');
const saleMessage = document.getElementById('saleMessage');
const saleMedicineId = document.getElementById('saleMedicineId');

let medicines = [];

const daysUntil = (isoDate) => {
  const now = new Date();
  now.setHours(0, 0, 0, 0);
  const expiry = new Date(isoDate);
  return Math.ceil((expiry - now) / (1000 * 60 * 60 * 24));
};

const loadMedicines = async (search = '') => {
  const query = search ? `?search=${encodeURIComponent(search)}` : '';
  const response = await fetch(`/api/medicines${query}`);
  medicines = await response.json();
  renderMedicines();
  renderMedicineDropdown();
};

const renderMedicines = () => {
  medicineTableBody.innerHTML = '';

  medicines.forEach((medicine) => {
    const row = document.createElement('tr');
    const expiryInDays = daysUntil(medicine.expiryDate);

    if (expiryInDays < 30) {
      row.classList.add('expiry-soon');
    } else if (medicine.quantity < 10) {
      row.classList.add('low-stock');
    }

    row.innerHTML = `
      <td>${medicine.fullName}</td>
      <td>${medicine.brand}</td>
      <td>${medicine.expiryDate}</td>
      <td>${medicine.quantity}</td>
      <td>$${Number(medicine.price).toFixed(2)}</td>
    `;

    medicineTableBody.appendChild(row);
  });
};

const renderMedicineDropdown = () => {
  saleMedicineId.innerHTML = medicines
    .map((medicine) => `<option value="${medicine.id}">${medicine.fullName} (${medicine.quantity} in stock)</option>`)
    .join('');
};

medicineForm.addEventListener('submit', async (event) => {
  event.preventDefault();
  const formData = new FormData(medicineForm);

  const payload = {
    fullName: formData.get('fullName'),
    brand: formData.get('brand'),
    notes: formData.get('notes') || '',
    expiryDate: formData.get('expiryDate'),
    quantity: Number(formData.get('quantity')),
    price: Number(formData.get('price'))
  };

  const response = await fetch('/api/medicines', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });

  if (response.ok) {
    medicineMessage.textContent = 'Medicine added successfully.';
    medicineForm.reset();
    await loadMedicines(searchInput.value);
  } else {
    const error = await response.json();
    medicineMessage.textContent = error.message || 'Unable to add medicine.';
  }
});

saleForm.addEventListener('submit', async (event) => {
  event.preventDefault();

  const payload = {
    medicineId: saleMedicineId.value,
    quantitySold: Number(document.getElementById('saleQuantity').value)
  };

  const response = await fetch('/api/sales', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });

  if (response.ok) {
    saleMessage.textContent = 'Sale recorded successfully.';
    saleForm.reset();
    await loadMedicines(searchInput.value);
  } else {
    const error = await response.json();
    saleMessage.textContent = error.message || 'Unable to record sale.';
  }
});

searchInput.addEventListener('input', async () => {
  await loadMedicines(searchInput.value);
});

loadMedicines();
