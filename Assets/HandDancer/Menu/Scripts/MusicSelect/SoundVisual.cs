using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundVisual : MonoBehaviour
{
    const int SAMPLE_SIZE = 1024;

    public float rmsValue;
    public float dbValue;
    public float pitchValue;

    public float backgroundIntensity;
    public Material backgroundMaterial;
    public Color minColor;
    public Color maxColor;

    public float maxVisualScale = 25f;
    public float visualModifier = 50f;
    public float smoothSpeed = 10f;
    public float keepPercentage = 0.5f;
    public Image spectrumObject;

    public AudioSource audioSource;
    float[] samples;
    float[] spectrum;
    float sampleRate;


    Image[] visualList;
    float[] visualScale;
    int amnVisual = 15;

    private void Start()
    {
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;

        SpawnCircle();
    }
    void SpawnCircle()
    {
        visualScale = new float[amnVisual];
        visualList = new Image[amnVisual];

        float dirAmt = (360f / amnVisual) / 2;
        int half = amnVisual / 2;
        float radius = 5.5f;

        for (int i = 0; i < amnVisual; i++)
        {
            Image sr = Instantiate(spectrumObject, transform);
            visualList[i] = sr;

            float dir = i <= half ? dirAmt * i : dirAmt * (half - i);
            Vector3 pos = transform.position + new Vector3(Mathf.Cos(dir * Mathf.Deg2Rad) * radius, Mathf.Sin(dir * Mathf.Deg2Rad) * radius, 0f);
            Quaternion rot = Quaternion.Euler(0, 0, dir);

            visualList[i].transform.position = pos;
            visualList[i].transform.rotation = rot;
        }
    }

    private void Update()
    {
        AnalyzeSound();
        UpdateVisual();
        UpdateBackground();
    }
    void UpdateVisual()
    {
        int visualIndex = 0;
        int spectrumIndex = 0;
        int averageSize = (int)((SAMPLE_SIZE * keepPercentage) / amnVisual);

        while (visualIndex < amnVisual)
        {
            int j = 0;
            float sum = 0;
            while (j < averageSize)
            {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
                j++;
            }

            float scaleY = sum / averageSize * visualModifier;
            visualScale[visualIndex] -= Time.deltaTime * smoothSpeed;

            if (visualScale[visualIndex] < scaleY)
                visualScale[visualIndex] = scaleY;

            if (visualScale[visualIndex] > maxVisualScale)
                visualScale[visualIndex] = maxVisualScale;

            visualList[visualIndex].rectTransform.sizeDelta = new Vector2(0.1f, 150f) + Vector2.right * visualScale[visualIndex];
            visualIndex++;
        }
    }
    void AnalyzeSound()
    {
        audioSource.GetOutputData(samples, 0);

        int i = 0;
        float sum = 0;
        for (; i < SAMPLE_SIZE; i++)
        {
            sum = samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);

        dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);

        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < SAMPLE_SIZE; i++)
        {
            if (!(spectrum[i] > maxV) || !(spectrum[i] > 0f))
                continue;

            maxV = spectrum[i];
            maxN = i;
        }

        float freqN = maxN;
        if (maxN > 0 && maxN < SAMPLE_SIZE - 1)
        {
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        pitchValue = freqN * (sampleRate / 2) / SAMPLE_SIZE;
    }

    void Average()
    {
        float average, std_deviation, variance = 0;
        float summ = 0;
        for (int idx = 0; idx < 1024; idx++) summ = summ + spectrum[idx];
        average = summ / 1024f;
        for (int idx = 0; idx < 1024; idx++) variance = variance + (spectrum[idx] - average) * (spectrum[idx] - average);
        variance = variance / 1024f;
        std_deviation = Mathf.Sqrt(variance);

        int a = 0, b = 0;
        while (true)
        {
            if (spectrum[b] <= std_deviation)
                spectrum[b] = spectrum[b] + std_deviation / 2;
            else if (b >= 1023)
                break;
            else
                b++;
        }
    }
    void UpdateBackground()
    {
        backgroundIntensity -= Time.deltaTime * 0.5f;
        if (backgroundIntensity < dbValue / 40f)
            backgroundIntensity = dbValue / 40f;

        backgroundMaterial.color = Color.Lerp(maxColor, minColor, -backgroundIntensity);
    }

    public void ChangeMusic(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
