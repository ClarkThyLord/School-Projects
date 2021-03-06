/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.c
  * @brief          : Main program body
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; Copyright (c) 2020 STMicroelectronics.
  * All rights reserved.</center></h2>
  *
  * This software component is licensed by ST under BSD 3-Clause license,
  * the "License"; You may not use this file except in compliance with the
  * License. You may obtain a copy of the License at:
  *                        opensource.org/licenses/BSD-3-Clause
  *
  ******************************************************************************
  */
/* USER CODE END Header */
/* Includes ------------------------------------------------------------------*/
#include "main.h"
#include "cmsis_os.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */

/* USER CODE END Includes */

/* Private typedef -----------------------------------------------------------*/
/* USER CODE BEGIN PTD */

/* USER CODE END PTD */

/* Private define ------------------------------------------------------------*/
/* USER CODE BEGIN PD */
/* USER CODE END PD */

/* Private macro -------------------------------------------------------------*/
/* USER CODE BEGIN PM */

/* USER CODE END PM */

/* Private variables ---------------------------------------------------------*/

UART_HandleTypeDef huart1;

osThreadId ToggleGreenHandle;
osThreadId ToggleRedHandle;
osThreadId ToggleYellowHandle;
osSemaphoreId blueBinarySemaphoreHandle;
/* USER CODE BEGIN PV */

/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
static void MX_GPIO_Init(void);
static void MX_USART1_UART_Init(void);
void StartToggleGreen(void const * argument);
void StartToggleRed(void const * argument);
void StartToggleYellow(void const * argument);
void shared_function();

int flag = 1;

/* USER CODE BEGIN PFP */

/* USER CODE END PFP */

/* Private user code ---------------------------------------------------------*/
/* USER CODE BEGIN 0 */

/* USER CODE END 0 */

/**
  * @brief  The application entry point.
  * @retval int
  */
int main(void)
{
  /* USER CODE BEGIN 1 */

  /* USER CODE END 1 */

  /* MCU Configuration--------------------------------------------------------*/

  /* Reset of all peripherals, Initializes the Flash interface and the Systick. */
  HAL_Init();

  /* USER CODE BEGIN Init */

  /* USER CODE END Init */

  /* Configure the system clock */
  SystemClock_Config();

  /* USER CODE BEGIN SysInit */

  /* USER CODE END SysInit */

  /* Initialize all configured peripherals */
  MX_GPIO_Init();
  MX_USART1_UART_Init();
  /* USER CODE BEGIN 2 */

  /* USER CODE END 2 */

  /* USER CODE BEGIN RTOS_MUTEX */
  /* add mutexes, ... */
  /* USER CODE END RTOS_MUTEX */

  /* Create the semaphores(s) */
  /* definition and creation of blueBinarySemaphore */
  osSemaphoreDef(blueBinarySemaphore);
  blueBinarySemaphoreHandle = osSemaphoreCreate(osSemaphore(blueBinarySemaphore), 1);

  /* USER CODE BEGIN RTOS_SEMAPHORES */
  /* add semaphores, ... */
  /* USER CODE END RTOS_SEMAPHORES */

  /* USER CODE BEGIN RTOS_TIMERS */
  /* start timers, add new ones, ... */
  /* USER CODE END RTOS_TIMERS */

  /* USER CODE BEGIN RTOS_QUEUES */
  /* add queues, ... */
  /* USER CODE END RTOS_QUEUES */

  /* Create the thread(s) */
  /* definition and creation of ToggleGreen */
  osThreadDef(ToggleGreen, StartToggleGreen, osPriorityNormal, 0, 128);
  ToggleGreenHandle = osThreadCreate(osThread(ToggleGreen), NULL);

  /* definition and creation of ToggleRed */
  osThreadDef(ToggleRed, StartToggleRed, osPriorityNormal, 0, 128);
  ToggleRedHandle = osThreadCreate(osThread(ToggleRed), NULL);

  /* definition and creation of ToggleYellow */
  osThreadDef(ToggleYellow, StartToggleYellow, osPriorityBelowNormal, 0, 128);
  ToggleYellowHandle = osThreadCreate(osThread(ToggleYellow), NULL);

  /* USER CODE BEGIN RTOS_THREADS */
  /* add threads, ... */
  /* USER CODE END RTOS_THREADS */

  /* Start scheduler */
  osKernelStart();

  /* We should never get here as control is now taken by the scheduler */
  /* Infinite loop */
  /* USER CODE BEGIN WHILE */
  while (1)
  {
    /* USER CODE END WHILE */

    /* USER CODE BEGIN 3 */
  }
  /* USER CODE END 3 */
}

/**
  * @brief System Clock Configuration
  * @retval None
  */
void SystemClock_Config(void)
{
  RCC_OscInitTypeDef RCC_OscInitStruct = {0};
  RCC_ClkInitTypeDef RCC_ClkInitStruct = {0};
  RCC_PeriphCLKInitTypeDef PeriphClkInitStruct = {0};

  /** Configure the main internal regulator output voltage
  */
  __HAL_RCC_PWR_CLK_ENABLE();
  __HAL_PWR_VOLTAGESCALING_CONFIG(PWR_REGULATOR_VOLTAGE_SCALE3);
  /** Initializes the RCC Oscillators according to the specified parameters
  * in the RCC_OscInitTypeDef structure.
  */
  RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSI;
  RCC_OscInitStruct.HSIState = RCC_HSI_ON;
  RCC_OscInitStruct.HSICalibrationValue = RCC_HSICALIBRATION_DEFAULT;
  RCC_OscInitStruct.PLL.PLLState = RCC_PLL_NONE;
  if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
  {
    Error_Handler();
  }
  /** Initializes the CPU, AHB and APB buses clocks
  */
  RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK|RCC_CLOCKTYPE_SYSCLK
                              |RCC_CLOCKTYPE_PCLK1|RCC_CLOCKTYPE_PCLK2;
  RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_HSI;
  RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
  RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV1;
  RCC_ClkInitStruct.APB2CLKDivider = RCC_HCLK_DIV1;

  if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_0) != HAL_OK)
  {
    Error_Handler();
  }
  PeriphClkInitStruct.PeriphClockSelection = RCC_PERIPHCLK_USART1;
  PeriphClkInitStruct.Usart1ClockSelection = RCC_USART1CLKSOURCE_PCLK2;
  if (HAL_RCCEx_PeriphCLKConfig(&PeriphClkInitStruct) != HAL_OK)
  {
    Error_Handler();
  }
}

/**
  * @brief USART1 Initialization Function
  * @param None
  * @retval None
  */
static void MX_USART1_UART_Init(void)
{

  /* USER CODE BEGIN USART1_Init 0 */

  /* USER CODE END USART1_Init 0 */

  /* USER CODE BEGIN USART1_Init 1 */

  /* USER CODE END USART1_Init 1 */
  huart1.Instance = USART1;
  huart1.Init.BaudRate = 115200;
  huart1.Init.WordLength = UART_WORDLENGTH_8B;
  huart1.Init.StopBits = UART_STOPBITS_1;
  huart1.Init.Parity = UART_PARITY_NONE;
  huart1.Init.Mode = UART_MODE_TX_RX;
  huart1.Init.HwFlowCtl = UART_HWCONTROL_NONE;
  huart1.Init.OverSampling = UART_OVERSAMPLING_16;
  huart1.Init.OneBitSampling = UART_ONE_BIT_SAMPLE_DISABLE;
  huart1.AdvancedInit.AdvFeatureInit = UART_ADVFEATURE_NO_INIT;
  if (HAL_UART_Init(&huart1) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN USART1_Init 2 */

  /* USER CODE END USART1_Init 2 */

}

/**
  * @brief GPIO Initialization Function
  * @param None
  * @retval None
  */
static void MX_GPIO_Init(void)
{
  GPIO_InitTypeDef GPIO_InitStruct = {0};

  /* GPIO Ports Clock Enable */
  __HAL_RCC_GPIOB_CLK_ENABLE();
  __HAL_RCC_GPIOC_CLK_ENABLE();
  __HAL_RCC_GPIOA_CLK_ENABLE();

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(GPIOB, GreenLED_Pin|RedLED_Pin|BlueLED_Pin, GPIO_PIN_RESET);

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(Pinout_GPIO_Port, Pinout_Pin, GPIO_PIN_RESET);

  /*Configure GPIO pins : GreenLED_Pin RedLED_Pin BlueLED_Pin */
  GPIO_InitStruct.Pin = GreenLED_Pin|RedLED_Pin|BlueLED_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOB, &GPIO_InitStruct);

  /*Configure GPIO pin : Pinout_Pin */
  GPIO_InitStruct.Pin = Pinout_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(Pinout_GPIO_Port, &GPIO_InitStruct);

}

/* USER CODE BEGIN 4 */

void shared_function() {
    if( flag == 1){
        flag = 0;
        HAL_Delay(3000);
        flag = 1;
        HAL_GPIO_WritePin(BlueLED_GPIO_Port, BlueLED_Pin, GPIO_PIN_RESET);
    } else {
        // Show an alert turning the blue led on
        HAL_GPIO_WritePin(BlueLED_GPIO_Port, BlueLED_Pin, GPIO_PIN_SET);
    }
}

/* USER CODE END 4 */

/* USER CODE BEGIN Header_StartToggleGreen */
/**
  * @brief  Function implementing the ToggleGreen thread.
  * @param  argument: Not used
  * @retval None
  */
/* USER CODE END Header_StartToggleGreen */
void StartToggleGreen(void const * argument)
{
  /* USER CODE BEGIN 5 */
  /* Infinite loop */
  for(;;)
  {
	HAL_GPIO_WritePin(GreenLED_GPIO_Port, GreenLED_Pin, GPIO_PIN_SET);
	osDelay(5000);
	taskDISABLE_INTERRUPTS();
//	taskENTER_CRITICAL();
//	shared_function();
//	taskEXIT_CRITICAL();
	osSemaphoreWait(blueBinarySemaphoreHandle, 100);
	shared_function();
	osSemaphoreRelease(blueBinarySemaphoreHandle);
	taskENABLE_INTERRUPTS();
	HAL_GPIO_WritePin(GreenLED_GPIO_Port, GreenLED_Pin, GPIO_PIN_RESET);
	osDelay(500);
  }
  /* USER CODE END 5 */
}

/* USER CODE BEGIN Header_StartToggleRed */
/**
* @brief Function implementing the ToggleRed thread.
* @param argument: Not used
* @retval None
*/
/* USER CODE END Header_StartToggleRed */
void StartToggleRed(void const * argument)
{
  /* USER CODE BEGIN StartToggleRed */
  /* Infinite loop */
  for(;;)
  {
	  HAL_GPIO_WritePin(RedLED_GPIO_Port, RedLED_Pin, GPIO_PIN_SET);
	  osDelay(2000);
	  taskDISABLE_INTERRUPTS();
//	  taskENTER_CRITICAL();
//	  shared_function();
//	  taskEXIT_CRITICAL();
	  osSemaphoreWait(blueBinarySemaphoreHandle, 100);
	  shared_function();
	  osSemaphoreRelease(blueBinarySemaphoreHandle);
	  taskENABLE_INTERRUPTS();
	  HAL_GPIO_WritePin(RedLED_GPIO_Port, RedLED_Pin, GPIO_PIN_RESET);
	  osDelay(500);
  }
  /* USER CODE END StartToggleRed */
}

/* USER CODE BEGIN Header_StartToggleYellow */
/**
* @brief Function implementing the ToggleYellow thread.
* @param argument: Not used
* @retval None
*/
/* USER CODE END Header_StartToggleYellow */
void StartToggleYellow(void const * argument)
{
  /* USER CODE BEGIN StartToggleYellow */
  /* Infinite loop */
  for(;;)
  {
	  HAL_GPIO_WritePin(Pinout_GPIO_Port, Pinout_Pin, GPIO_PIN_SET);
	  osDelay(10000);
	  taskDISABLE_INTERRUPTS();
//	  taskENTER_CRITICAL();
//	  shared_function();
//	  taskEXIT_CRITICAL();
	  osSemaphoreWait(blueBinarySemaphoreHandle, 100);
	  shared_function();
	  osSemaphoreRelease(blueBinarySemaphoreHandle);
	  taskENABLE_INTERRUPTS();
	  HAL_GPIO_WritePin(Pinout_GPIO_Port, Pinout_Pin, GPIO_PIN_RESET);
	  osDelay(500);
  }
  /* USER CODE END StartToggleYellow */
}

/**
  * @brief  Period elapsed callback in non blocking mode
  * @note   This function is called  when TIM1 interrupt took place, inside
  * HAL_TIM_IRQHandler(). It makes a direct call to HAL_IncTick() to increment
  * a global variable "uwTick" used as application time base.
  * @param  htim : TIM handle
  * @retval None
  */
void HAL_TIM_PeriodElapsedCallback(TIM_HandleTypeDef *htim)
{
  /* USER CODE BEGIN Callback 0 */

  /* USER CODE END Callback 0 */
  if (htim->Instance == TIM1) {
    HAL_IncTick();
  }
  /* USER CODE BEGIN Callback 1 */

  /* USER CODE END Callback 1 */
}

/**
  * @brief  This function is executed in case of error occurrence.
  * @retval None
  */
void Error_Handler(void)
{
  /* USER CODE BEGIN Error_Handler_Debug */
  /* User can add his own implementation to report the HAL error return state */

  /* USER CODE END Error_Handler_Debug */
}

#ifdef  USE_FULL_ASSERT
/**
  * @brief  Reports the name of the source file and the source line number
  *         where the assert_param error has occurred.
  * @param  file: pointer to the source file name
  * @param  line: assert_param error line source number
  * @retval None
  */
void assert_failed(uint8_t *file, uint32_t line)
{
  /* USER CODE BEGIN 6 */
  /* User can add his own implementation to report the file name and line number,
     tex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
  /* USER CODE END 6 */
}
#endif /* USE_FULL_ASSERT */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
