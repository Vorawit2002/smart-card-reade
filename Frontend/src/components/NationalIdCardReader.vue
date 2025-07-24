<template>
  <v-container class="py-8 px-4" fluid>
    <v-row justify="center">
      <v-col cols="12" md="10" lg="8">
        <!-- Main Card -->
        <v-card elevation="2" rounded="xl" class="main-card pa-6 text-center">
          <!-- Title Section -->
          <div class="title-section mb-8">
            <div class="icon-wrapper mb-4">
              <i class="ri-bank-card-line" style="font-size: 64px; color: #1976d2"></i>
            </div>
            <v-card-title class="title-text text-primary mb-2">
              อ่านข้อมูลบัตรประชาชน
            </v-card-title>
            <v-card-subtitle class="subtitle-text mb-2">
              ระบบอ่านข้อมูลบัตรประชาชนอัตโนมัติ
            </v-card-subtitle>
            <div class="description-text text-grey-darken-1">
              เสียบบัตรประชาชนเข้าเครื่องอ่าน ระบบจะอ่านข้อมูลอัตโนมัติ
            </div>
          </div>

          <v-card-text>
            <!-- Status Display -->
            <div class="status-container mb-6">
              <div v-if="loading" class="status-loading text-center">
                <div class="loading-icon mb-3">
                  <i class="ri-loader-4-line rotating" style="font-size: 48px; color: #1976d2"></i>
                </div>
                <div class="text-h6 text-primary mb-2">กำลังอ่านข้อมูล...</div>
                <v-progress-linear
                  color="primary"
                  indeterminate
                  rounded
                  height="4"
                  class="mb-2"
                ></v-progress-linear>
                <div class="text-caption text-grey-darken-1">
                  กรุณารอสักครู่ ระบบกำลังประมวลผลข้อมูล...
                </div>
              </div>

              <div v-else-if="!cardData && !error" class="status-waiting text-center">
                <div class="waiting-icon mb-3">
                  <i class="ri-bank-card-line" style="font-size: 48px; color: #9e9e9e"></i>
                </div>
                <div class="text-h6 text-grey-darken-1 mb-2">รอการเสียบบัตร</div>
                <div class="text-body-2 text-grey-darken-2">เสียบบัตรประชาชนเข้าเครื่องอ่านบัตรประชาชนบนเครื่องอ่าน</div>
              </div>
            </div>

            <!-- Error Alert -->
            <v-alert
              v-if="error"
              type="error"
              variant="tonal"
              border="start"
              class="mb-6"
              closable
              rounded="lg"
            >
              <template v-slot:prepend>
                <i class="ri-error-warning-line" style="font-size: 20px"></i>
              </template>
              <div class="text-body-1 font-weight-medium">เกิดข้อผิดพลาด</div>
              <div class="text-body-2 mt-1">{{ error }}</div>
            </v-alert>

            <!-- Data Display Card -->
            <v-card v-if="cardData" elevation="1" rounded="xl" class="data-card mt-6">
              <!-- Card Header -->
              <div class="card-header pa-4 text-center">
                <i class="ri-checkbox-circle-line mb-2" style="font-size: 40px; color: #4caf50"></i>
                <div class="text-h6 font-weight-bold text-success mb-1">อ่านข้อมูลสำเร็จ</div>
                <div class="text-caption text-grey-darken-1">ข้อมูลบัตรประชาชนแสดงด้านล่าง</div>
              </div>

              <v-divider></v-divider>

              <v-row no-gutters>
                <!-- Photo Section -->
                <v-col cols="12" md="4" class="photo-section pa-6">
                  <div class="photo-container">
                    <div class="photo-frame">
                      <v-avatar size="140" rounded="lg" class="photo-avatar">
                        <v-img
                          v-if="cardData.photoBase64"
                          :src="`data:image/jpeg;base64,${cardData.photoBase64}`"
                          alt="รูปภาพเจ้าของบัตร"
                          cover
                        ></v-img>
                        <i v-else class="ri-user-line" style="font-size: 140px; color: #e0e0e0"></i>
                      </v-avatar>
                    </div>
                    <div class="photo-label mt-3">
                      <v-chip
                        color="primary"
                        variant="tonal"
                        size="small"
                        prepend-icon="ri-camera-line"
                        rounded="pill"
                      >
                        รูปถ่าย
                      </v-chip>
                    </div>
                  </div>
                </v-col>

                <!-- Data Section -->
                <v-col cols="12" md="8" class="data-section pa-4">
                  <div class="data-grid">
                    <v-card
                      v-for="(item, index) in cardInfo"
                      :key="index"
                      class="data-item pa-3 mb-2"
                      elevation="0"
                      rounded="lg"
                      variant="tonal"
                      color="grey-lighten-5"
                    >
                      <div class="d-flex align-center">
                        <div class="item-icon mr-3">
                          <i
                            class="ri-record-circle-line"
                            style="font-size: 20px; color: #1976d2"
                          ></i>
                        </div>
                        <div class="flex-grow-1">
                          <div
                            class="item-label text-caption text-grey-darken-2 font-weight-bold mb-1"
                          >
                            {{ item.label }}
                          </div>
                          <div class="item-value text-body-1 text-grey-darken-4">
                            {{ item.value || '-' }}
                          </div>
                        </div>
                      </div>
                    </v-card>
                  </div>
                </v-col>
              </v-row>

              <!-- Footer -->
              <v-divider></v-divider>
              <div class="card-footer pa-3 text-center">
                <v-chip
                  color="success"
                  variant="tonal"
                  size="small"
                  prepend-icon="ri-shield-check-line"
                  rounded="pill"
                >
                  ข้อมูลถูกต้องและปลอดภัย
                </v-chip>
              </div>
            </v-card>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import axios from 'axios'

interface CardData {
  citizenId: string
  titleNameTh: string
  firstNameTh: string
  lastNameTh: string
  titleNameEn: string
  firstNameEn: string
  lastNameEn: string
  birthDate: string
  gender: string
  address: string
  issueDate: string
  expireDate: string
  photoBase64?: string
}

const cardData = ref<CardData | null>(null)
const loading = ref(false)
const error = ref<string | null>(null)
let pollingInterval: number | null = null

// จัดรูปแบบวันที่ให้เป็นไทย (วัน/เดือน/ปี พ.ศ.)
const formatThaiDate = (isoDate: string): string => {
  try {
    const date = new Date(isoDate)
    return new Intl.DateTimeFormat('th-TH', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    }).format(date)
  } catch {
    return isoDate
  }
}

// ข้อมูลที่จะแสดง
const cardInfo = computed(() => {
  if (!cardData.value) return []
  const data = cardData.value
  return [
    { field: 'เลขประจำตัวประชาชน', label: 'เลขประจำตัวประชาชน', value: data.citizenId },
    {
      field: 'ชื่อ-นามสกุล (ไทย)',
      label: 'ชื่อ-นามสกุล (ไทย)',
      value: `${data.titleNameTh} ${data.firstNameTh} ${data.lastNameTh}`,
    },
    {
      field: 'ชื่อ-นามสกุล (อังกฤษ)',
      label: 'ชื่อ-นามสกุล (อังกฤษ)',
      value: `${data.titleNameEn} ${data.firstNameEn} ${data.lastNameEn}`,
    },
    { field: 'วันเกิด', label: 'วันเกิด', value: formatThaiDate(data.birthDate) },
    { field: 'เพศ', label: 'เพศ', value: data.gender },
    { field: 'ที่อยู่', label: 'ที่อยู่', value: data.address },
    { field: 'วันออกบัตร', label: 'วันออกบัตร', value: formatThaiDate(data.issueDate) },
    { field: 'วันหมดอายุ', label: 'วันหมดอายุ', value: formatThaiDate(data.expireDate) },
  ]
})

// ตรวจสอบสถานะบัตร
const checkCardStatus = async () => {
  try {
    const response = await axios.get('http://localhost:5003/NationalId/status', {
      timeout: 5000,
    })
    return response.data.cardPresent || false
  } catch {
    return false
  }
}

// อ่านข้อมูลจาก backend
const readCard = async () => {
  if (loading.value) return

  loading.value = true
  error.value = null

  try {
    const response = await axios.get<CardData>('http://localhost:5003/NationalId/read', {
      timeout: 15000,
    })
    cardData.value = response.data
  } catch (err: unknown) {
    if (axios.isAxiosError(err)) {
      error.value =
        err.response?.data?.message || err.response?.data || err.message || 'การเชื่อมต่อล้มเหลว'
    } else {
      error.value = 'เกิดข้อผิดพลาดที่ไม่คาดคิด'
    }
  } finally {
    loading.value = false
  }
}

// เริ่มการตรวจสอบบัตรอัตโนมัติ
const startCardPolling = () => {
  pollingInterval = setInterval(async () => {
    if (loading.value) return

    const cardPresent = await checkCardStatus()
    if (cardPresent && !cardData.value) {
      await readCard()
    } else if (!cardPresent && cardData.value) {
      // ถอดบัตรแล้ว รีเซ็ตข้อมูล
      cardData.value = null
      error.value = null
    }
  }, 2000) // ตรวจสอบทุก 2 วินาที
}

// หยุดการตรวจสอบ
const stopCardPolling = () => {
  if (pollingInterval) {
    clearInterval(pollingInterval)
    pollingInterval = null
  }
}

// เริ่มต้นเมื่อ component mount
onMounted(() => {
  startCardPolling()
})

// หยุดเมื่อ component unmount
onUnmounted(() => {
  stopCardPolling()
})
</script>

<style scoped>
/* Main Card Styling */
.main-card {
  background: white;
  border: 1px solid rgba(0, 0, 0, 0.08);
}

/* Title Section */
.title-section {
  margin-bottom: 2rem;
}

.title-text {
  font-size: 1.8rem !important;
  font-weight: 700 !important;
  color: #1976d2 !important;
}

.subtitle-text {
  font-size: 1rem !important;
  font-weight: 500 !important;
  color: #424242 !important;
}

.description-text {
  font-size: 0.9rem;
  line-height: 1.5;
  max-width: 500px;
  margin: 0 auto;
}

/* Button Styling */
.read-button {
  font-weight: 600 !important;
  font-size: 1rem !important;
  text-transform: none !important;
  letter-spacing: 0.25px;
  transition: all 0.2s ease;
}

.read-button:hover {
  transform: translateY(-1px);
}

.status-indicator {
  max-width: 300px;
  margin: 0 auto;
}

/* Data Card */
.data-card {
  background: white;
  border: 1px solid rgba(0, 0, 0, 0.08);
}

.card-header {
  background: #fafafa;
}

/* Photo Section */
.photo-section {
  background: #fafafa;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  border-right: 1px solid rgba(0, 0, 0, 0.08);
}

.photo-container {
  text-align: center;
}

.photo-frame {
  position: relative;
  display: inline-block;
  padding: 4px;
  background: #1976d2;
  border-radius: 12px;
}

.photo-avatar {
  border: 3px solid white;
}

/* Data Section */
.data-section {
  background: white;
}

.data-grid {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.data-item {
  border: 1px solid rgba(0, 0, 0, 0.05);
  transition: all 0.2s ease;
}

.data-item:hover {
  border-color: #1976d2;
  transform: translateX(4px);
}

.item-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  background: rgba(25, 118, 210, 0.1);
  border-radius: 6px;
}

.item-label {
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-size: 0.75rem;
}

.item-value {
  word-wrap: break-word;
  overflow-wrap: break-word;
  line-height: 1.4;
  font-weight: 500;
}

.card-footer {
  background: #fafafa;
}

/* Rotating animation for loading icon */
.rotating {
  animation: rotate 1s linear infinite;
}

@keyframes rotate {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}
@media (max-width: 960px) {
  .title-text {
    font-size: 1.6rem !important;
  }

  .photo-section {
    border-right: none;
    border-bottom: 1px solid rgba(0, 0, 0, 0.08);
  }
}

@media (max-width: 600px) {
  .main-card {
    margin: 0 8px;
  }

  .title-text {
    font-size: 1.4rem !important;
  }

  .subtitle-text {
    font-size: 0.9rem !important;
  }

  .photo-avatar {
    width: 120px !important;
    height: 120px !important;
  }

  .data-item {
    padding: 12px !important;
  }
}
</style>
