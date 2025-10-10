/**
 * Get a lighter version of a color by blending it with white
 * @param hexCode - Hex color code (with or without #)
 * @param amount - Amount to lighten (0-1), default 0.7
 * @returns Light version of the color in hex format
 */
export const getLightColor = (hexCode: string | null | undefined, amount = 0.7): string => {
  if (!hexCode) return '#e0e0e0'
  
  // Remove # if present
  const hex = hexCode.replace('#', '')
  
  // Convert to RGB
  const r = parseInt(hex.substring(0, 2), 16)
  const g = parseInt(hex.substring(2, 4), 16)
  const b = parseInt(hex.substring(4, 6), 16)
  
  // Create lighter version (blend with white)
  const lighten = (value: number) => Math.round(value + (255 - value) * amount)
  
  const lightR = lighten(r)
  const lightG = lighten(g)
  const lightB = lighten(b)
  
  return `#${lightR.toString(16).padStart(2, '0')}${lightG.toString(16).padStart(2, '0')}${lightB.toString(16).padStart(2, '0')}`
}

/**
 * Determine the appropriate text color (dark or light) based on background color
 * @param hexCode - Hex color code (with or without #)
 * @param useDarkText - Force dark text for light palette
 * @returns Hex color code for text
 */
export const getTextColor = (hexCode: string | null | undefined, useDarkText = true): string => {
  if (!hexCode) return '#000000'
  
  if (useDarkText) {
    // For light palette, always use dark text
    return '#424242'
  }
  
  // Calculate relative luminance to determine if we need light or dark text
  const hex = hexCode.replace('#', '')
  const r = parseInt(hex.substring(0, 2), 16)
  const g = parseInt(hex.substring(2, 4), 16)
  const b = parseInt(hex.substring(4, 6), 16)
  
  // Calculate relative luminance
  const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255
  
  // Return dark text for light backgrounds, light text for dark backgrounds
  return luminance > 0.5 ? '#424242' : '#ffffff'
}
