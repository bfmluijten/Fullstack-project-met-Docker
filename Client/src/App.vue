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
        <button @click="savePatient" :disabled="loading">{{ editingId == null ? 'Create' : 'Save' }}</button>
        <button @click="cancelEdit" type="button">Cancel</button>
        <button v-if="editingId == null" @click="fetchPatients" type="button">Refresh</button>
      </div>
      <div v-if="error" class="error">{{ error }}</div>
    </section>

    <section class="list">
      <h2>List</h2>
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
                <button @click="startEdit(patient)">Edit</button>
                <button @click="deletePatient(patient)">Delete</button>
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
}

.controls {
  margin-bottom: 0.5rem;
}

.error {
  color: #b00020;
}

.note {
  margin-top: 1rem;
  font-size: 0.9rem;
  color: #555;
}

button[disabled] {
  opacity: 0.6;
}
</style>
