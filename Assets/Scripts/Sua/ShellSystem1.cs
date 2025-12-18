using UnityEngine;
using System;
using System.Collections.Generic;
using Extension.Test;

namespace Game.Player.Combat
{
    public enum ShellType
    {
        None,
        Shell1,
        Shell2,
        Shell3
    }

    public class ShellSystem : MonoBehaviour
    {

        public event Action<ShellType> OnShellEquipped;
        public event Action<ShellType> OnShellUnequipped;

        private HashSet<ShellType> m_equippedShells = new HashSet<ShellType>();
        private const int MAX_EQUIPPED_SHELLS = 2;

        private Dictionary<ShellType, bool> m_shellUnlocked = new Dictionary<ShellType, bool> {
            { ShellType.Shell1, false },
            { ShellType.Shell2, false },
            { ShellType.Shell3, false }
        };

        public bool EquipShell(ShellType pShellType)
        {
            if (pShellType == ShellType.None)
            {
                return false;
            }

            if (!m_shellUnlocked[pShellType])
            {
                Debug.LogWarning($"[ShellSystem] {pShellType} is not unlocked!");
                return false;
            }

            if (m_equippedShells.Contains(pShellType))
            {
                Debug.LogWarning($"[ShellSystem] {pShellType} is already equipped!");
                return false;
            }

            if (m_equippedShells.Count >= MAX_EQUIPPED_SHELLS)
            {
                Debug.LogWarning($"[ShellSystem] Cannot equip more than {MAX_EQUIPPED_SHELLS} shells!");
                return false;
            }

            m_equippedShells.Add(pShellType);
            OnShellEquipped?.Invoke(pShellType);
            Debug.Log($"[ShellSystem] {pShellType} equipped!");
            return true;
        }

        public bool UnequipShell(ShellType pShellType)
        {
            if (!m_equippedShells.Contains(pShellType))
            {
                Debug.LogWarning($"[ShellSystem] {pShellType} is not equipped!");
                return false;
            }

            m_equippedShells.Remove(pShellType);
            OnShellUnequipped?.Invoke(pShellType);
            Debug.Log($"[ShellSystem] {pShellType} unequipped!");
            return true;
        }

        public bool IsShellEquipped(ShellType pShellType)
        {
            return m_equippedShells.Contains(pShellType);
        }

        public void UnlockShell(ShellType pShellType)
        {
            if (pShellType != ShellType.None)
            {
                m_shellUnlocked[pShellType] = true;
                Debug.Log($"[ShellSystem] {pShellType} unlocked!");
            }
        }

        [TestMethod("Test Unlock All Shells")]
        private void TestUnlockAllShells()
        {
            UnlockShell(ShellType.Shell1);
            UnlockShell(ShellType.Shell2);
            UnlockShell(ShellType.Shell3);
        }

        [TestMethod("Test Equip Shell1")]
        private void TestEquipShell1()
        {
            EquipShell(ShellType.Shell1);
        }

        [TestMethod("Test Unequip Shell1")]
        private void TestUnequipShell1()
        {
            UnequipShell(ShellType.Shell1);
        }

        public int GetEquippedShellCount => m_equippedShells.Count;
    }
}