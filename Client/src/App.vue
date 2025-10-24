<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface Patient {
  id?: number | string
  naam?: string
  adres?: string
  geboortejaar?: number
}

const patients = ref<Patient[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

// Form state for create / edit
const form = ref<Partial<Patient>>({ naam: '', adres: '', geboortejaar: undefined })
const editingId = ref<number | string | null>(null)

const startCreate = () => {
  editingId.value = null
  form.value = { naam: '', adres: '', geboortejaar: undefined }
}

const startEdit = (p: Patient) => {
  editingId.value = p.id ?? null
  form.value = { naam: p.naam, adres: p.adres, geboortejaar: p.geboortejaar }
}

const cancelEdit = () => {
  editingId.value = null
  form.value = { naam: '', adres: '', geboortejaar: undefined }
  error.value = null
}

const fetchPatients = async () => {
  loading.value = true
  error.value = null
  try {
    // Use a relative path so the request goes to the same origin the app was served from.
    // This avoids scheme/host mismatches (http vs https) and works with the nginx proxy.
    const res = await fetch('/api/Patients')
    if (!res.ok) {
      throw new Error(`Server responded ${res.status} ${res.statusText}`)
    }
    const data = await res.json()
    // Expecting an array; guard against single-object responses.
    patients.value = Array.isArray(data) ? data : [data]
  } catch (err: any) {
    error.value = err?.message ?? String(err)
    patients.value = []
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  void fetchPatients()
})

const savePatient = async () => {
  error.value = null
  if (!form.value.naam || String(form.value.naam).trim() === '') {
    error.value = 'Naam is verplicht.'
    return
  }

  loading.value = true
  try {
    const payload = {
      id: editingId.value ?? undefined,
      naam: form.value.naam,
      adres: form.value.adres,
      geboortejaar: form.value.geboortejaar
    }

    if (editingId.value == null) {
      // Create
      const res = await fetch('/api/Patients', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      })
      if (!res.ok) throw new Error(`Create failed ${res.status} ${res.statusText}`)
    } else {
      // Update
      const res = await fetch(`/api/Patients/${editingId.value}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      })
      if (!res.ok) throw new Error(`Update failed ${res.status} ${res.statusText}`)
    }

    await fetchPatients()
    cancelEdit()
  } catch (err: any) {
    error.value = err?.message ?? String(err)
  } finally {
    loading.value = false
  }
}

const deletePatient = async (p: Patient) => {
  if (!p.id) return
  const ok = confirm(`Verwijder patiënt ${p.naam ?? p.id}?`)
  if (!ok) return

  loading.value = true
  error.value = null
  try {
    const res = await fetch(`/api/Patients/${p.id}`, { method: 'DELETE' })
    if (!res.ok) throw new Error(`Delete failed ${res.status} ${res.statusText}`)
    await fetchPatients()
  } catch (err: any) {
    error.value = err?.message ?? String(err)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="app-root">
    <h1>Patients</h1>

    <section class="form">
      <h2>{{ editingId == null ? 'Create patient' : 'Edit patient' }}</h2>
      <div class="form-row">
        <label>Naam</label>
        <input v-model="form.naam" placeholder="Naam" />
      </div>
      <div class="form-row">
        <label>Adres</label>
        <input v-model="form.adres" placeholder="Adres" />
      </div>
      <div class="form-row">
        <label>Geboortejaar</label>
        <input v-model.number="form.geboortejaar" type="number" placeholder="Geboortejaar" />
      </div>

      <div class="form-actions">
        <button class="icon-btn primary" @click="savePatient" :disabled="loading" :title="editingId == null ? 'Maak' : 'Opslaan'" :aria-label="editingId == null ? 'Maak' : 'Opslaan'">
          <!-- plus / save icon -->
          <svg viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg" aria-hidden>
            <path d="M19 13H13V19H11V13H5V11H11V5H13V11H19V13Z" fill="currentColor"/>
          </svg>
          <span class="btn-label">{{ editingId == null ? 'Maak' : 'Opslaan' }}</span>
        </button>

        <button class="icon-btn" @click="cancelEdit" type="button" title="Annuleer" aria-label="Annuleer">
          <!-- x / cancel icon -->
          <svg viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg" aria-hidden>
            <path d="M18.3 5.71L12 12l6.3 6.29-1.41 1.42L10.59 13.41 4.29 19.71 2.88 18.3 9.17 12 2.88 5.71 4.29 4.29 10.59 10.59 16.88 4.29z" fill="currentColor"/>
          </svg>
        </button>
      </div>
      <div v-if="error" class="error">{{ error }}</div>
    </section>

    <section class="list">
      <div class="list-header">
        <h2>List</h2>
        <button class="icon-btn" @click="fetchPatients" type="button" title="Ververs" aria-label="Ververs">
          <!-- refresh icon -->
          <svg viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg" aria-hidden>
            <path d="M17.65 6.35A7.95 7.95 0 0 0 12 4V1L7 6l5 5V7c2.76 0 5 2.24 5 5a5 5 0 0 1-5 5 5 5 0 0 1-4.9-4H5.1A7 7 0 0 0 12 20a7 7 0 0 0 5.65-11.65z" fill="currentColor"/>
          </svg>
        </button>
      </div>
      <div v-if="loading">Loading patients…</div>
      <div v-else>
        <ul>
          <li v-if="patients.length === 0">No patients found.</li>
          <li v-for="patient in patients" :key="patient.id">
            <div class="item">
              <div class="info">
                <strong>{{ patient.naam ?? 'Unknown' }}</strong>
                <div class="meta">{{ patient.adres ?? '' }} <span v-if="patient.geboortejaar">- {{ patient.geboortejaar }}</span></div>
              </div>
              <div class="actions">
                <button class="icon-btn edit" @click="startEdit(patient)" title="Bewerk" aria-label="Bewerk">
                  <!-- pencil icon -->
                  <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" aria-hidden>
                    <path d="M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25z" fill="currentColor"/>
                    <path d="M20.71 7.04a1.003 1.003 0 0 0 0-1.42l-2.34-2.34a1.003 1.003 0 0 0-1.42 0l-1.83 1.83 3.75 3.75 1.84-1.82z" fill="currentColor"/>
                  </svg>
                </button>
                <button class="icon-btn delete" @click="deletePatient(patient)" title="Verwijder" aria-label="Verwijder">
                  <!-- trash icon -->
                  <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" aria-hidden>
                    <path d="M6 19a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V7H6v12z" fill="currentColor"/>
                    <path d="M19 4h-3.5l-1-1h-5l-1 1H5v2h14V4z" fill="currentColor"/>
                  </svg>
                </button>
              </div>
            </div>
          </li>
        </ul>
      </div>
    </section>
  </div>
</template>

<style scoped>
.app-root {
  font-family: system-ui, -apple-system, 'Segoe UI', Roboto, 'Helvetica Neue', Arial;
  padding: 1rem;
  max-width: 900px;
  margin: 0 auto;
}

.form {
  background: #fff;
  padding: 0.75rem;
  border-radius: 8px;
  box-shadow: 0 1px 2px rgba(0,0,0,0.03);
  margin-bottom: 1rem;
}
.form-row {
  display: flex;
  gap: 0.5rem;
  align-items: center;
  margin-bottom: 0.5rem;
}
.form-row label {
  width: 120px;
  font-weight: 600;
}
.form-row input {
  flex: 1;
  padding: 0.4rem 0.5rem;
  border: 1px solid #ddd;
  border-radius: 6px;
}
.form-actions {
  display: flex;
  gap: 0.5rem;
  align-items: center;
  margin-top: 0.5rem;
  justify-content: flex-end; /* place buttons to the right */
}

.list {
  background: #fff;
  padding: 0.5rem;
  border-radius: 8px;
  box-shadow: 0 1px 2px rgba(0,0,0,0.03);
}
.list-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
}
.list-header h2 { margin: 0; }
.list ul {
  list-style: none;
  margin: 0;
  padding: 0;
}
.list li {
  margin: 0;
}
.item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.6rem 0.5rem;
  border-bottom: 1px solid #f0f0f0;
}
.item:last-child { border-bottom: none; }
.info {
  flex: 1 1 auto;
  min-width: 0;
}
.info strong { display:block; font-size: 1rem; }
.meta { color: #666; font-size: 0.9rem; margin-top: 0.25rem; }
.actions {
  display: flex;
  gap: 0.5rem;
  align-items: center;
  margin-left: 1rem;
  flex: 0 0 auto;
}

/* Icon buttons */
.icon-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  border: 1px solid transparent;
  background: #f5f5f5;
  padding: 0.35rem 0.45rem;
  border-radius: 6px;
  cursor: pointer;
  color: #333;
}
.icon-btn svg { width: 18px; height: 18px; display: block; }
.icon-btn:hover { filter: brightness(0.96); }
.icon-btn:active { transform: translateY(1px); }
.icon-btn.primary { background: #0b66ff; color: #fff; }
.icon-btn.edit { background: #eaf4ff; color: #0b66ff; border-color: rgba(11,102,255,0.12); }
.icon-btn.delete { background: #fff5f5; color: #d32f2f; border-color: rgba(211,47,47,0.08); }

/* Most icon buttons are icon-only — hide label unless .primary */
.icon-btn .btn-label { display: none; }
.icon-btn.primary .btn-label { display: inline-block; margin-left: 0.35rem; }

.error { color: #b00020; margin-top: 0.5rem; }

button[disabled] { opacity: 0.6; cursor: not-allowed; }

@media (max-width: 640px) {
  .form-row { flex-direction: column; align-items: stretch; }
  .form-row label { width: auto; }
  .actions { gap: 0.35rem; }
  .form-actions { justify-content: flex-start; flex-wrap: wrap; }
}
</style>
