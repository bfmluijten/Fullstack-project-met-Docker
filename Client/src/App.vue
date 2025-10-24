<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface Patient {
  id?: number | string
  firstName?: string
  lastName?: string
  [key: string]: any
}

const patients = ref<Patient[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

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
</script>

<template>
  <div class="app-root">
    <h1>Patients</h1>

    <div class="controls">
      <button @click="fetchPatients" :disabled="loading">Refresh</button>
    </div>

    <div v-if="loading">Loading patients…</div>
    <div v-else-if="error" class="error">Error: {{ error }}</div>
    <ul v-else>
      <li v-if="patients.length === 0">No patients found.</li>
      <li v-for="patient in patients" :key="patient.id">
        <strong>{{ patient.naam ?? 'Unknown' }}</strong>
        <span> - {{ patient.adres }}</span>
        <span> - {{ patient.geboortejaar }}</span>
        <small v-if="patient.id"> - ID: {{ patient.id }}</small>
      </li>
    </ul>

  <p class="note">If you see CORS or network errors, make sure the API at <code>/api/Patients</code> is reachable from the client and allows requests from this origin.</p>
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
